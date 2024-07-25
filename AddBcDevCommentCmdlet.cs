namespace ConvertXliffToBcDevComments;

public class AddBcDevCommentCmdlet : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0)]
    public string ObjectPath { get; set; }

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public XliffTranslation[] Translations { get; set; }





    // Skip target langauge en-us
    // SKip untranslated
    // FOrce voor overschrijven
}