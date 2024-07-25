namespace ConvertXliffToBcDevComments;

public class AddBcDevCommentCmdlet : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0)]
    public string ObjectPath { get; set; }

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public XliffTranslation[] Translations { get; set; }

    [Parameter()]
    public SwitchParameter Force { get; set; }

    protected List<XliffTranslation> CachedTranslations = [];

    protected override void ProcessRecord()
    {
        CachedTranslations.AddRange(Translations);
    }

    protected override void EndProcessing()
    {
        // Cache objects


        // Write dirty objects
    }



    // Skip target langauge en-us
    // SKip untranslated
    // FOrce voor overschrijven
}