using System.Xml.Linq;

namespace ConvertXliffToBcDevComments;

[Cmdlet(VerbsCommon.Get, Nouns.XliffTranslation)]
[OutputType(typeof(XliffTranslation))]
public class GetXliffTranslationCmdlet : PSCmdlet
{
    [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
    [Alias("FullName")]
    public string[] Path { get; set; }

    [Parameter()]
    public SwitchParameter Recurse { get; set; }

    protected List<string> CachedPaths = [];

    protected override void ProcessRecord()
    {
        CachedPaths.AddRange(
            Path
                .SelectMany(p => GetResolvedProviderPathFromPSPath(p, out _))
                .SelectMany(p => Directory.Exists(p) ? Directory.GetFiles(p, "*.xlf") : [p])
        );
    }

    protected override void EndProcessing()
    {
        XNamespace @namespace = "urn:oasis:names:tc:xliff:document:1.2";

        WriteObject(
            CachedPaths
                .Select(p => new { Path = p, Document = XDocument.Load(p) })
                .Select(p => new
                {
                    p.Path,
                    p.Document,
                    SourceLanguage = p.Document.Root.Element(@namespace + "file").Attribute("source-language").Value,
                    TargetLanguage = p.Document.Root.Element(@namespace + "file").Attribute("target-language").Value,
                    TranslationUnits = p.Document.Root.Descendants(@namespace + "trans-unit")
                })
                .SelectMany(
                    p => p.TranslationUnits.Select(u => new
                    {
                        p.Path,
                        p.SourceLanguage,
                        p.TargetLanguage,
                        Source = u.Element(@namespace + "source").Value,
                        Target = u.Element(@namespace + "target")?.Value,
                        TargetState = u.Element(@namespace + "target")?.Attribute("state")?.Value,
                        RawContext = u.Elements(@namespace + "note").SingleOrDefault(e => e.Attribute("from")?.Value == "Xliff Generator")?.Value
                    }
                    )
                )
                .Select(u => new XliffTranslation()
                {
                    XliffPath = u.Path,
                    SourceLanguage = u.SourceLanguage,
                    TargetLanguage = u.TargetLanguage,
                    TargetState = TranslationStateParser.Parse(u.TargetState),
                    Source = u.Source,
                    Target = u.Target,
                    RawContext = u.RawContext
                }),
            true
        );
    }
}
