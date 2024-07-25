using System.Collections.ObjectModel;

namespace ConvertXliffToBcDevComments;

public enum ContextElementType
{
    Enum,
    EnumValue,
    Property
}

public class ContextElement
{
    public required ContextElementType Type { get; init; }
    public required string Name { get; init; }
}

public class Context
{
    public Context(string rawContext)
    {


        var elements = rawContext.Split(" - Property ");

        Property = new ContextElement()
        {
            Type = ContextElementType.Property,
            Name = elements[1]
        };

        rawContext = elements[0];
    }

    public ContextElement Object { get; }
    public Collection<ContextElement> Subobjects { get; } = [];
    public ContextElement Property { get; }
}