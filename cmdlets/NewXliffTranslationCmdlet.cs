using System.Management.Automation.Language;

namespace ConvertXliffToBcDevComments;

[Cmdlet(VerbsCommon.New, Nouns.XliffTranslation)]
[OutputType(typeof(XliffTranslation))]
public class NewXliffTranslationCmdlet : Cmdlet
{
    [Parameter()] public string XliffPath { get; set; }
    [Parameter()] public string SourceLanguage { get; set; } = Facts.BaseLanguage;
    [Parameter(Mandatory = true)] public string TargetLanguage { get; set; }
    [Parameter()] public string Source { get; set; }
    [Parameter(Mandatory = true)] public string Target { get; set; }
    [Parameter()] public TranslationState? TargetState { get; set; }
    [Parameter(Mandatory = true)] public string RawContext { get; set; } // FIXME: Wordt: Context

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
                TargetState = TargetState,
                RawContext = RawContext
            }
        );
    }
}