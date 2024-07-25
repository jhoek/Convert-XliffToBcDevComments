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

    protected override void ProcessRecord()
    {
        CachedTranslations.AddRange(Translations);
    }

    protected override void EndProcessing()
    {
        CachedTranslations
            .Where(t => t.TargetLanguage != Facts.BaseLanguage)
            .Where(t => IncludeState.Contains(t.TargetState ?? TranslationState.Translated));

        // FIXME: Cache objects
        // FIXME: Apply translation if missing or -Force, warn if context not found
        // FIXME: Write dirty objects
    }
}