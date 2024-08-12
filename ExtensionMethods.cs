using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ConvertXliffToBcDevComments;

public static class ExtensionMethods
{
    public static SyntaxNode FindSyntaxNodeByContext(this IEnumerable<ContextElement> contextElements, SyntaxNode context)
    {
        SyntaxNode newContext = contextElements.First().Type switch
        {
            ContextElementType.Table => (context as CompilationUnitSyntax).Objects.OfType<TableSyntax>().SingleOrDefault(t => t.Name.Matches(contextElements.First().Name)) ?? throw new ArgumentException(),
            ContextElementType.Field => (context as TableSyntax).Fields.Fields.SingleOrDefault(f => f.Name.Matches(contextElements.First().Name)) ?? throw new ArgumentException(),

        };

        return contextElements.Skip(1).FindSyntaxNodeByContext(newContext);
    }

    public static bool Matches(this string value1, string value2) => value1.Equals(value2, StringComparison.InvariantCultureIgnoreCase);
    public static bool Matches(this IdentifierNameSyntax value1, string value2) => value1.Identifier.ValueText.Matches(value2);

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

    public static void WriteObject(this object value, Cmdlet cmdlet, bool enumerateCollection = true) => cmdlet.WriteObject(value, enumerateCollection);
}