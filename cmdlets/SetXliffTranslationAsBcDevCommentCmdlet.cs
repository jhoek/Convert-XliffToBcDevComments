using System.Collections;
using System.Collections.Frozen;
using System.Runtime.InteropServices;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

namespace ConvertXliffToBcDevComments;

[Cmdlet(VerbsCommon.Set, "XliffTranslationAsBcDevComment")]
[OutputType(typeof(XliffTranslation))]
public class SetXliffTranslationAsBcDevCommentCmdlet : PSCmdlet
{
    public class SetXliffTranslationAsBcDevCommentRewriter : SyntaxRewriter
    {
        public SetXliffTranslationAsBcDevCommentRewriter(IEnumerable<XliffTranslation> translations)
        {
            Translations = translations;
        }

        public IEnumerable<XliffTranslation> Translations { get; init; }
        public SwitchParameter Force { get; set; }
        public Action<string> WriteVerbose { get; set; }
        public Action<XliffTranslation> WriteProcessedTranslation { get; set; }

        public override SyntaxNode VisitLabel(LabelSyntax node)
        {
            var contextString = node.ContextString();
            WriteVerbose($"Context string is {contextString}");

            var translation = Translations.SingleOrDefault(t => t.Context.Matches(contextString));

            if (translation is not null)
            {
                WriteVerbose($"Found translation '{translation.Target}'");

                var oldLabelPropertyValueProperties = node.Properties;
                var oldLabelPropertyValues = oldLabelPropertyValueProperties?.Values ?? new SeparatedSyntaxList<IdentifierEqualsLiteralSyntax>();
                var oldCommentsProperty = oldLabelPropertyValues.SingleOrDefault(v => v.Identifier.ValueText.Matches("Comment"));
                var oldOtherProperties = oldLabelPropertyValues.Where(v => !v.Identifier.ValueText.Matches("Comment"));
                var oldCommentsPropertyValue = oldCommentsProperty?.Literal.ToFullString().UnquoteLiteral();

                var developerComments = new DeveloperComments(oldCommentsPropertyValue);
                var targetLanguagePresentInDevComments = developerComments.ContainsLanguageCode(translation.TargetLanguage);
                var translationsAlreadyMatch = developerComments.Get(translation.TargetLanguage) == translation.Target;

                var shouldSet = (!targetLanguagePresentInDevComments || Force) && !translationsAlreadyMatch;
                var shouldEmit = shouldSet || translationsAlreadyMatch;

                WriteVerbose($"Target language {translation.TargetLanguage} already present: {targetLanguagePresentInDevComments} ({developerComments.Get(translation.Target)}); Force: {Force}");

                if (shouldSet)
                {
                    WriteVerbose($"Comment property should be set (translation was missing or -Force was specified)");

                    developerComments.Set(translation.TargetLanguage, translation.Target);
                    var newCommentsPropertyValue = developerComments.ToString();
                    var newCommentsProperty = SyntaxFactory.IdentifierEqualsLiteral("Comment", SyntaxFactory.StringLiteralValue(SyntaxFactory.Literal(newCommentsPropertyValue)));
                    var newLabelPropertyValues = new SeparatedSyntaxList<IdentifierEqualsLiteralSyntax>().AddRange(oldOtherProperties.Prepend(newCommentsProperty));
                    var newLabelPropertyValueProperties = SyntaxFactory.CommaSeparatedIdentifierEqualsLiteralList(newLabelPropertyValues);
                    node = SyntaxFactory.Label(node.LabelText, SyntaxFactory.Token(SyntaxKind.CommaToken), newLabelPropertyValueProperties).NormalizeWhiteSpace();
                }

                if (shouldEmit)
                    WriteProcessedTranslation?.Invoke(translation);
            }

            return base.VisitLabel(node);
        }
    }

    [Parameter(Mandatory = true, Position = 0)]
    public string[] ObjectPath { get; set; }

    [Parameter()]
    public SwitchParameter Recurse { get; set; }

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public XliffTranslation[] Translations { get; set; }

    [Parameter()]
    public TranslationState[] IncludeState { get; set; } = [TranslationState.Final, TranslationState.Translated, TranslationState.SignedOff];

    [Parameter()]
    public SwitchParameter Force { get; set; }

    [Parameter()]
    public SwitchParameter PassThru { get; set; }

    protected List<XliffTranslation> CachedTranslations = [];

    protected SearchOption SearchOption =>
        Recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

    protected IEnumerable<string> ObjectFilePaths =>
        ObjectPath
            .SelectMany(o => GetResolvedProviderPathFromPSPath(o, out _))
            .SelectMany(p => Directory.Exists(p) ? Directory.GetFiles(p, "*.al", SearchOption) : [p]);

    protected override void ProcessRecord()
    {
        CachedTranslations.AddRange(Translations);
    }

    protected override void EndProcessing()
    {
        var translations = CachedTranslations
            .Where(t => t.TargetLanguage != Facts.BaseLanguage)
            .Where(t => IncludeState.Contains(t.TargetState ?? TranslationState.Translated));

        var rewriter = new SetXliffTranslationAsBcDevCommentRewriter(translations)
        {
            WriteVerbose = WriteVerbose,
            Force = Force
        };

        if (PassThru) rewriter.WriteProcessedTranslation = WriteObject;

        ObjectFilePaths
            .ToList()
            .ForEach(p =>
            {
                var compilationUnit = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(p), p).GetRoot();
                compilationUnit = rewriter.Visit(compilationUnit);
                File.WriteAllText(p, compilationUnit.ToFullString());
            });
    }
}