using System.Reflection.Emit;
using System.Xml.XPath;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace ConvertXliffToBcDevComments;

[Cmdlet(VerbsDiagnostic.Test, Nouns.BcDevComment)]
[OutputType(typeof(bool))]
public class TestBcDevCommentCmdlet : PSCmdlet
{
    public class TestBcDevCommentRewriter : SyntaxRewriter
    {
        public TestBcDevCommentRewriter(params string[] expectedLanguages)
        {
            ExpectedLanguages.AddRange(expectedLanguages);
        }

        public List<string> ExpectedLanguages { get; } = new List<string>();
        public string Separator { get; set; } = "|";
        public Action<string> WriteVerbose { get; set; }
        public Action<string> WriteWarning { get; set; }
        public bool AnyProblemsFound { get; protected set; }

        public override SyntaxNode VisitLabel(LabelSyntax node)
        {
            var labelText = node.LabelText.Value.ValueText.UnquoteLiteral();
            var containingObject = node.GetContainingObjectSyntax();
            var containingObjectText = $"{containingObject.GetType().Name.RegexReplace("Syntax$", "")} {containingObject.Name.Identifier.ValueText}";

            if (IsLocked(node))
            {
                WriteVerbose?.Invoke($"Label '{labelText}' in {containingObjectText} is locked and will not be analyzed further.");
                return base.VisitLabel(node);
            }

            var comment = GetComment(node);

            if (comment is null)
            {
                AnyProblemsFound = true;
                WriteWarning?.Invoke($"Developer comment expected but not found for label '{labelText}' in {containingObjectText}.");
                return base.VisitLabel(node);
            }

            var developerComments = new DeveloperComments(comment.UnquoteLiteral(), Separator).Where(c => c.HasLanguageCode);
            var developerCommentLanguages = developerComments.Select(c => c.LanguageCode);

            if (!developerCommentLanguages.Any())
            {
                AnyProblemsFound = true;
                WriteWarning?.Invoke($"Developer comments with language codes expected but not found for label '{labelText}' in {containingObjectText}.");
                return base.VisitLabel(node);
            }

            WriteVerbose?.Invoke($"Developer comments found for languages {string.Join(",", developerCommentLanguages)} in label '{labelText}' in {containingObjectText}");

            ExpectedLanguages
                .Where(e => !developerCommentLanguages.Contains(e))
                .ToList()
                .ForEach(e =>
                    {
                        AnyProblemsFound = true;
                        WriteWarning?.Invoke($"Developer comment expected for language {e} but not found for label '{labelText}' in {containingObjectText}.");
                    }
                );

            var maxLength = GetMaxLength(node);

            if (maxLength == 0)
            {
                WriteVerbose?.Invoke($"No maximum length found for label '{labelText}' in {containingObjectText}.");
                return base.VisitLabel(node);
            }

            developerComments
                .Where(c => c.Value.Length > maxLength)
                .ToList()
                .ForEach(c =>
                    {
                        AnyProblemsFound = true;
                        WriteWarning($"Developer comment for language {c.LanguageCode} exceeds the maximum length of {maxLength} for label '{labelText}' in {containingObjectText}.");
                    });

            return base.VisitLabel(node);
        }

        protected bool IsLocked(LabelSyntax labelSyntax)
        {
            if (labelSyntax.Properties is null)
                return false;

            var lockedProperty = labelSyntax.Properties.Values.SingleOrDefault(v => v.Identifier.Matches(LabelPropertyHelper.Locked));

            if (lockedProperty is null)
                return false;

            var isLocked = (lockedProperty.Literal as BooleanLiteralValueSyntax).Value.IsKind(SyntaxKind.TrueKeyword);

            return isLocked;
        }

        protected string GetComment(LabelSyntax labelSyntax)
        {
            if (labelSyntax.Properties is null)
                return null;

            var commentProperty = labelSyntax.Properties.Values.SingleOrDefault(v => v.Identifier.Matches(LabelPropertyHelper.Comment));

            if (commentProperty is null)
                return null;

            var comment = (commentProperty.Literal as StringLiteralValueSyntax).Value.ValueText;

            return comment;
        }

        protected int GetMaxLength(LabelSyntax labelSyntax)
        {
            if (labelSyntax.Properties is null)
                return 0;

            var maxLengthProperty = labelSyntax.Properties.Values.SingleOrDefault(v => v.Identifier.Matches(LabelPropertyHelper.MaxLength));

            if (maxLengthProperty is null)
                return 0;

            var maxLength = (int)(maxLengthProperty.Literal as Int32SignedLiteralValueSyntax).Number.Value;

            return maxLength;
        }
    }

    [Parameter(Mandatory = true, Position = 0)]
    public string[] ObjectPath { get; set; }

    [Parameter()]
    public SwitchParameter Recurse { get; set; }

    [Parameter(Mandatory = true)]
    public string[] ExpectedLanguage { get; set; }

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    public string DevCommentSeparator { get; set; } = "|";

    protected SearchOption SearchOption =>
        Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

    protected IEnumerable<string> ObjectFilePaths =>
    ObjectPath
        .SelectMany(o => GetResolvedProviderPathFromPSPath(o, out _))
        .SelectMany(p => Directory.Exists(p) ? Directory.GetFiles(p, "*.al", SearchOption) : [p]);

    protected override void EndProcessing()
    {
        var rewriter = new TestBcDevCommentRewriter(ExpectedLanguage)
        {
            WriteVerbose = WriteVerbose,
            WriteWarning = WriteWarning,
            Separator = DevCommentSeparator
        };

        ObjectFilePaths
            .ToList()
            .ForEach(p =>
            {
                WriteVerbose($"Examining {p}");
                var compilationUnit = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(p), p).GetRoot();
                compilationUnit = rewriter.Visit(compilationUnit);
            });

        WriteObject(!rewriter.AnyProblemsFound);
    }
}