namespace ConvertXliffToBcDevComments;

public class XliffTranslation
{
    public required string XliffPath { get; init; }
    public required string SourceLanguage { get; init; }
    public required string TargetLanguage { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
    public required string TargetState { get; init; }
    public required string Context { get; init; }
}