using System.Collections;
using System.Collections.ObjectModel;

namespace ConvertXliffToBcDevComments;

public enum ContextElementType
{
    Table,
    Field,
    Enum,
    EnumValue,
    Property,
    NamedType
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






    }

    public IEnumerable<string> Elements(string rawContext)
    {



    }
}