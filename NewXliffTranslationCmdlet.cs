using System.Management.Automation.Language;

namespace ConvertXliffToBcDevComments;

[Cmdlet(VerbsCommon.New, Nouns.XliffTranslation)]
[OutputType(typeof(XliffTranslation))]
public class NewXliffTranslationCmdlet : Cmdlet
{
    [Parameter()] public string XliffPath { get; set; }
    [Parameter()] public string SourceLanguage { get; set; } = "en-US";
    [Parameter()] public string TargetLanguage { get; set; }
    [Parameter()] public string Source { get; set; }
    [Parameter()] public string Target { get; set; }
    [Parameter()] public string RawContext { get; set; }

    protected override void EndProcessing()
    {
        WriteObject(
            new XliffTranslation()
            {
                XliffPath = XliffPath,
                SourceLanguage = SourceLanguage,
                TargetLanguage = TargetLanguage,
                Source = Source,
                Target = Target,
                RawContext = RawContext,
                Context = new Context(RawContext)
            }
        );
    }
}