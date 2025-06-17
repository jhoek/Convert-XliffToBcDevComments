using System.Management.Automation.Language;
using UncommonSense.PowerShell.Maml.Attributes;

namespace ConvertXliffToBcDevComments;

[CmdletDescription("Creates a user-defined translation entry.")]
[Cmdlet(VerbsCommon.New, Nouns.XliffTranslation)]
[OutputType(typeof(XliffTranslation))]
public class NewXliffTranslationCmdlet : Cmdlet
{
    [Parameter()] public string XliffPath { get; set; }
    [Parameter()] public string SourceLanguage { get; set; } = Facts.BaseLanguage;
    [Parameter(Mandatory = true)] public string TargetLanguage { get; set; }
    [Parameter(Mandatory = true)] public string ID { get; set; }
    [Parameter()] public string Source { get; set; }
    [Parameter(Mandatory = true)] public string Target { get; set; }
    [Parameter()] public TranslationState? TargetState { get; set; }
    [Parameter(Mandatory = true)] public string Context { get; set; }

    protected override void EndProcessing()
    {
        WriteObject(
            new XliffTranslation()
            {
                XliffPath = XliffPath,
                SourceLanguage = SourceLanguage,
                TargetLanguage = TargetLanguage,
                ID = ID,
                Source = Source,
                Target = Target,
                TargetState = TargetState,
                Context = Context
            }
        );
    }
}