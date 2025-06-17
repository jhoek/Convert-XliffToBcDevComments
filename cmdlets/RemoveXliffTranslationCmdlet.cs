using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.XPath;
using UncommonSense.PowerShell.Maml.Attributes;

namespace ConvertXliffToBcDevComments;

[CmdletDescription("Removes translations from their respective translation files.")]
[Cmdlet(VerbsCommon.Remove, Nouns.XliffTranslation)]
public class RemoveXliffTranslationCmdlet : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
    public XliffTranslation[] InputObject { get; set; }

    protected List<XliffTranslation> CachedTranslations = [];

    public readonly XNamespace @namespace = "urn:oasis:names:tc:xliff:document:1.2";

    protected override void ProcessRecord()
    {
        CachedTranslations.AddRange(
            InputObject
                .Where(i => !string.IsNullOrEmpty(i.XliffPath))
        );
    }

    protected override void EndProcessing()
    {
        WriteVerbose($"Received {CachedTranslations.Count} translations to try and remove from the XLIFF file.");

        CachedTranslations
            .GroupBy(t => t.XliffPath)
            .ToList()
            .ForEach(g => ProcessTranslationFile(g, g.Key));
    }

    protected void ProcessTranslationFile(IEnumerable<XliffTranslation> translations, string filePath)
    {
        var document = XDocument.Load(filePath);
        var translationUnits = document.Descendants(@namespace + "trans-unit");

        if (ProcessTranslations(translations, translationUnits))
            document.Save(filePath);
    }

    protected bool ProcessTranslations(IEnumerable<XliffTranslation> translations, IEnumerable<XElement> translationUnits) =>
        translations
            .Where(t => ProcessTranslation(t, translationUnits))
            .ToList()
            .Any();

    protected bool ProcessTranslation(XliffTranslation translation, IEnumerable<XElement> translationUnits)
    {
        var translationUnit = translationUnits
            .SingleOrDefault(u => u
                .Elements(@namespace + "note")
                .Where(n => n.Attribute("from").Value == "Xliff Generator")
                .Where(n => n.Value == translation.Context)
                .Any()
            );

        if (translationUnit is null)
        {
            WriteVerbose($"- A translation-unit could not be found for translation {translation.Context}.");
            return false;
        }

        translationUnit.Remove();
        WriteVerbose($"- A translation-unit for translation {translation.Context} was found and removed.");

        return true;
    }
}