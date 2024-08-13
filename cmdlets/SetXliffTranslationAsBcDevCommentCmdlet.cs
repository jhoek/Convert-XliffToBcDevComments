using System.Collections;
using System.Collections.Frozen;
using Microsoft.Dynamics.Nav.CodeAnalysis.InternalSyntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ConvertXliffToBcDevComments;

[Cmdlet(VerbsCommon.Set, "XliffTranslationAsBcDevComment")]
[OutputType(typeof(FileInfo))]
public class SetXliffTranslationAsBcDevCommentCmdlet : PSCmdlet
{
    public class SetXliffTranslationAsBcDevCommentRewriter : SyntaxRewriter
    {
        public SetXliffTranslationAsBcDevCommentRewriter(IEnumerable<XliffTranslation> translations)
        {
            Translations = translations;
        }

        public IEnumerable<XliffTranslation> Translations { get; init; }
        public Action<string> WriteVerbose { get; set; }

        public override SyntaxNode VisitProperty(PropertySyntax node)
        {
            if (node.PropertyKind().IsTranslatableProperty())
            {
                var translation = Translations.SingleOrDefault(t => t.RawContext.Matches(node.ContextString()));

                if (translation is not null)
                {
                    // Parse existing commetns
                    // Skip if already present and not -Force
                    // Apply if found
                    // Remove from translations
                }
            }

            return base.VisitProperty(node);
        }
    }

    [Parameter(Mandatory = true, Position = 0)]
    public string ObjectPath { get; set; }

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
        GetResolvedProviderPathFromPSPath(ObjectPath, out _)
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

        translations.ToList().ForEach(t => WriteVerbose($"{t.RawContext} = {t.Target}"));

        var rewriter = new SetXliffTranslationAsBcDevCommentRewriter(translations) { WriteVerbose = WriteVerbose };

        ObjectFilePaths
            .ToList()
            .ForEach(p =>
            {
                var compilationUnit = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(p), p).GetRoot();
                compilationUnit = rewriter.Visit(compilationUnit);
                File.WriteAllText(p, compilationUnit.ToFullString());

                if (PassThru) WriteObject(new FileInfo(p));
            });
    }
}