using System.Management.Automation.Language;
using UncommonSense.PowerShell.Maml.Attributes;

namespace ConvertXliffToBcDevComments;

[CmdletDescription("Creates a user-defined translation entry.")]
[Cmdlet(VerbsCommon.New, Nouns.XliffTranslation)]
[OutputType(typeof(XliffTranslation))]
public class NewXliffTranslationCmdlet : Cmdlet
{
    [ParameterDescription("Path to the XLIFF file")][Parameter()] public string XliffPath { get; set; }
    [ParameterDescription("Source language for the translation; defaults to 'en-US'")][Parameter()] public string SourceLanguage { get; set; } = Facts.BaseLanguage;
    [ParameterDescription("Target language for the translation")][Parameter(Mandatory = true)] public string TargetLanguage { get; set; }
    [ParameterDescription("Unique ID for the translation")][Parameter(Mandatory = true)] public string ID { get; set; }
    [ParameterDescription("Source text for the translation")][Parameter()] public string Source { get; set; }
    [ParameterDescription("Target text for the translation")][Parameter(Mandatory = true)] public string Target { get; set; }
    [ParameterDescription("State of the translation target text")][Parameter()] public TranslationState? TargetState { get; set; }
    [ParameterDescription("Context for the translation")][Parameter(Mandatory = true)] public string Context { get; set; }

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