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
        public TranslationState[] StatesToProcess { get; set; }
        public TranslationState[] StatesToEmit { get; set; }
        public SwitchParameter Force { get; set; }
        public Action<string> WriteVerbose { get; set; }
        public Action<XliffTranslation> WriteProcessedTranslation { get; set; }

        public override SyntaxNode VisitLabel(LabelSyntax node)
        {
            var contextString = node.ContextString();
            WriteVerbose($"- Context string is {contextString}");

            var translation = Translations.SingleOrDefault(t => t.Context.Matches(contextString));

            if (translation is not null)
            {
                WriteVerbose($"  - Found translation '{translation.Target}'");
                var shouldProcess = StatesToProcess.Contains(translation.TargetState ?? TranslationState.Translated);
                var shouldEmit = StatesToEmit.Contains(translation.TargetState ?? TranslationState.Translated);

                if (shouldProcess)
                {
                    var oldLabelPropertyValueProperties = node.Properties;
                    var oldLabelPropertyValues = oldLabelPropertyValueProperties?.Values ?? new SeparatedSyntaxList<IdentifierEqualsLiteralSyntax>();
                    var oldCommentsProperty = oldLabelPropertyValues.SingleOrDefault(v => v.Identifier.ValueText.Matches("Comment"));
                    var oldOtherProperties = oldLabelPropertyValues.Where(v => !v.Identifier.ValueText.Matches("Comment"));
                    var oldCommentsPropertyValue = oldCommentsProperty?.Literal.ToFullString().UnquoteLiteral();

                    var developerComments = new DeveloperComments(oldCommentsPropertyValue);
                    var targetLanguagePresentInDevComments = developerComments.ContainsLanguageCode(translation.TargetLanguage);
                    var translationsAlreadyMatch = developerComments.Get(translation.TargetLanguage) == translation.Target;

                    shouldProcess = (!targetLanguagePresentInDevComments || Force) && !translationsAlreadyMatch;

                    WriteVerbose($"  - Target language {translation.TargetLanguage} already present: {targetLanguagePresentInDevComments} ({developerComments.Get(translation.Target)}); Force: {Force}");

                    if (targetLanguagePresentInDevComments && !Force && !translationsAlreadyMatch)
                    {
                        shouldEmit = false; // Replacing translation in dev comment was skipped, keep translation in xliff file
                        WriteVerbose($"  - This entry will not be emitted since we didn't use it in the dev comments.");
                    }

                    if (shouldProcess)
                    {
                        WriteVerbose($"  - Comment property should be set (translation was missing or -Force was specified)");

                        developerComments.Set(translation.TargetLanguage, translation.Target);
                        var newCommentsPropertyValue = developerComments.ToString();
                        var newCommentsProperty = SyntaxFactory.IdentifierEqualsLiteral("Comment", SyntaxFactory.StringLiteralValue(SyntaxFactory.Literal(newCommentsPropertyValue)));
                        var newLabelPropertyValues = new SeparatedSyntaxList<IdentifierEqualsLiteralSyntax>().AddRange(oldOtherProperties.Prepend(newCommentsProperty));
                        var newLabelPropertyValueProperties = SyntaxFactory.CommaSeparatedIdentifierEqualsLiteralList(newLabelPropertyValues);
                        node = SyntaxFactory.Label(node.LabelText, SyntaxFactory.Token(SyntaxKind.CommaToken), newLabelPropertyValueProperties).NormalizeWhiteSpace();
                    }
                }
                else
                {
                    WriteVerbose($"  - Not processing, because state is {translation.TargetState}.");
                }

                if (shouldEmit)
                {
                    WriteProcessedTranslation?.Invoke(translation);
                }
                else
                {
                    WriteVerbose("  - Not emitting.");
                }
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
    [Alias("IncludeState")] // Previous name, for backward compatibility
    public TranslationState[] StateToProcess { get; set; } = [TranslationState.Final, TranslationState.Translated, TranslationState.SignedOff];

    [Parameter()]
    public TranslationState[] StateToEmit { get; set; } = [TranslationState.Final, TranslationState.NeedsTranslation, TranslationState.New, TranslationState.SignedOff, TranslationState.Translated];

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
            .Where(t => t.TargetLanguage != Facts.BaseLanguage);

        var rewriter = new SetXliffTranslationAsBcDevCommentRewriter(translations)
        {
            StatesToProcess = StateToProcess,
            StatesToEmit = StateToEmit,
            WriteVerbose = WriteVerbose,
            Force = Force
        };

        if (PassThru) rewriter.WriteProcessedTranslation = WriteObject;

        ObjectFilePaths
            .ToList()
            .ForEach(p =>
            {
                WriteVerbose($"Examining {p}");
                var compilationUnit = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(p), p).GetRoot();
                compilationUnit = rewriter.Visit(compilationUnit);
                File.WriteAllText(p, compilationUnit.ToFullString());
            });
    }
}