using System.Runtime.CompilerServices;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ConvertXliffToBcDevComments;

public static class ExtensionMethods
{
    public static SyntaxNode Resolve(this IEnumerable<SyntaxNode> syntaxNodes, IEnumerable<ContextElement> contextElements, Action<string> writeVerbose = null)
    {
        writeVerbose?.Invoke($"Looking for a {contextElements.First().Type} with name {contextElements.First().Name} in an enumerable of {syntaxNodes.Count()} syntax nodes.");

        switch (contextElements.First().Type)
        {
            case ContextElementType.Table:
                var table = syntaxNodes.OfType<TableSyntax>().Where(t => t.Name.Identifier.ValueText.Matches(contextElements.First().Name));
                return table.Resolve(contextElements.Skip(1));
            case ContextElementType.Field:
                var field = syntaxNodes.OfType<FieldSyntax>().Where(f => f.Name.Identifier.ValueText.Matches(contextElements.First().Name));
                return field.Resolve(contextElements.Skip(1));
            default:
                throw new ArgumentOutOfRangeException(nameof(contextElements), $"Don't know how to resolve a context element of type {contextElements.First().Type}");
        }
    }

    public static bool Matches(this string value1, string value2) => value1.Equals(value2, StringComparison.InvariantCultureIgnoreCase);

    public static SyntaxNode Resolve(this SyntaxNode syntaxNode, IEnumerable<ContextElement> contextElements, Action<string> writeVerbose = null)
    {
        if (!contextElements.Any())
        {
            writeVerbose?.Invoke("No more context elements found.");
            return syntaxNode;
        }

        writeVerbose?.Invoke($"Looking for a {contextElements.First().Type} with name {contextElements.First().Name} within a syntax node of type {syntaxNode.GetType().Name}");

        switch (contextElements.First().Type)
        {
            case ContextElementType.Field:
                return (syntaxNode as TableSyntax).Fields.Fields.Resolve(contextElements);
            default:
                throw new ArgumentOutOfRangeException(nameof(contextElements), $"Don't know how to resolve a context element of type {contextElements.First().Type}");
        }
    }
}