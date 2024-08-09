using System.Collections.Frozen;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ConvertXliffToBcDevComments;

[Cmdlet(VerbsCommon.Set, "XliffTranslationAsBcDevComment")]
public class SetXliffTranslationAsBcDevCommentCmdlet : PSCmdlet
{
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

        if (!translations.Any())
            return;

        var objects = ObjectFilePaths
            .Select(p => new
            {
                Path = p,
                Object = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(p), p).GetRoot().ChildNodes().Cast<ObjectSyntax>().SingleOrDefault()
            })
            .Where(o => o.Object is not null)
            .Select(o => o.Object)
            .ToList(); // FIXME: for now

        objects.Resolve(translations.First().Context.TakeWhile(c => c.Type != ContextElementType.Property).ToList());


        // WriteObject(objects.SelectMany(o => o.Object).FindFromContext(CachedTranslations.First().Context)); // etc.

        // FIXME: Loop through translations:
        // FIXME: - Find object and subobject based on translation context
        // FIXME: - Apply translation if missing or -Force, warn if context not found
        // FIXME: Write dirty objects
    }


}