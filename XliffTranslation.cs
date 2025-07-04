namespace ConvertXliffToBcDevComments;

public class XliffTranslation
{
    public string XliffPath { get; init; }
    public required string SourceLanguage { get; init; }
    public required string TargetLanguage { get; init; }
    public required string ID { get; init; }
    public required string Source { get; init; }
    public required string Target { get; init; }
    public TranslationState? TargetState { get; init; }
    public required string Context { get; init; } // Enum Team Hierarchy - EnumValue Managing Board & Executive Secretariat - Property Caption
    public override string ToString() => $"'{Source}' = '{Target}'";
}