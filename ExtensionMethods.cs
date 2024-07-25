using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ConvertXliffToBcDevComments;

public static class ExtensionMethods
{
    public static ObjectSyntax FindFromContext(this IEnumerable<ObjectSyntax> objects, IEnumerable<ContextElement> context, Action<string> writeWarning = null)
    {
        var objectsOfType = context.First().Type switch
        {
            ContextElementType.Table => objects.OfType<TableSyntax>(),
            _ => throw new ArgumentOutOfRangeException()
        };

        var result = objectsOfType.SingleOrDefault(o => o.Name.Identifier.ValueText.Equals(context.First().Name));

        if (result is null) writeWarning?.Invoke($"{context.First().Type} {context.First().Name} could not be found.");

        return result;
    }
}