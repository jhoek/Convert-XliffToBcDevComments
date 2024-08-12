using System.Collections;
using System.Collections.Frozen;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ConvertXliffToBcDevComments;

[Cmdlet(VerbsCommon.Set, "XliffTranslationAsBcDevComment")]
public class SetXliffTranslationAsBcDevCommentCmdlet : PSCmdlet
{
    public class SetXliffTranslationAsBcDevCommentRewriter : SyntaxRewriter
    {
        public SetXliffTranslationAsBcDevCommentRewriter(IEnumerable<XliffTranslation> translations)
        {
            Translations = translations;
        }

        public IEnumerable<XliffTranslation> Translations { get; init; }

        public override SyntaxNode VisitProperty(PropertySyntax node)
        {
            // Build context string
            // Skip if already present and not -Force
            // Find context string in translations
            // Apply if found
            // Remove from translations

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

        WriteVerbose($"{translations.Count()} translation found.");

        var compilationUnits = ObjectFilePaths
            .Select(p => new
            {
                Path = p,
                CompilationUnit = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(p), p).GetRoot(),
            })
            .Select(p => new
            {
                p.Path,
                p.CompilationUnit,
                Objects = p.CompilationUnit.ChildNodes().OfType<ObjectSyntax>()
            })
            .ToList();

        compilationUnits.SelectMany(c => c.Objects).ToList().ForEach(o => WriteVerbose(o.Name.Identifier.ValueText));

        translations
            .First() // FIXME
            .Context
            .FindSyntaxNodeByContext(compilationUnits.Select(c => c.CompilationUnit))
            .WriteObject(this);




        // FIXME: Loop through translations:
        // FIXME: - Find object and subobject based on translation context
        // FIXME: - Apply translation if missing or -Force, warn if context not found
        // FIXME: Write dirty objects
    }


}