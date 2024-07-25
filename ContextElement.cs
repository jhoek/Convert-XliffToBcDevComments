namespace ConvertXliffToBcDevComments;

public enum ContextElementType
{
    Enum,
    EnumValue,
    Caption
}

public class ContextElement
{
    public required ContextElementType Type { get; init; }
    public required string Name { get; init; }
}