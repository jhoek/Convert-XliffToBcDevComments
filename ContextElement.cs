namespace ConvertXliffToBcDevComments;

public class ContextElement
{
    public ContextElement(ContextElementType type, string name)
    {
        Type = type;
        Name = name;
    }

    public ContextElementType Type { get; init; }
    public string Name { get; init; }

    public override string ToString() =>
        $"{Type} {Name}";
}