using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ConvertXliffToBcDevComments;

public static class ExtensionMethods
{
    public static IEnumerable<SyntaxNode> FindSyntaxNodeByContext(this IEnumerable<ContextElement> contextElements, IEnumerable<SyntaxNode> syntaxNodes) =>
        syntaxNodes.Select(n => contextElements.FindSyntaxNodeByContext(n));

    public static SyntaxNode FindSyntaxNodeByContext(this IEnumerable<ContextElement> contextElements, SyntaxNode context)
    {


        SyntaxNode newContext = contextElements.First().Type switch
        {
            ContextElementType.Table => (context as CompilationUnitSyntax).Objects.OfType<TableSyntax>().SingleOrDefault(t => t.Name.Unquoted().Matches(contextElements.First().Name)) ?? throw new ArgumentException($"Couldn't find table {contextElements.First().Name}"),
            ContextElementType.Field => (context as TableSyntax).Fields.Fields.SingleOrDefault(f => f.Name.Unquoted().Matches(contextElements.First().Name)) ?? throw new ArgumentException($"Couldn't find field {contextElements.First().Name}"),
            ContextElementType.Property => null,
            _ => throw new ArgumentException(contextElements.First().Type.ToString())
        };

        if (newContext != null)
            return contextElements.Skip(1).FindSyntaxNodeByContext(newContext);
        else
            return null;
    }

    public static bool Matches(this string value1, string value2) => value1.Equals(value2, StringComparison.InvariantCultureIgnoreCase);
    public static bool Matches(this IdentifierNameSyntax value1, string value2) => value1.Identifier.ValueText.Matches(value2);
    public static void WriteObject(this object value, Cmdlet cmdlet, bool enumerateCollection = true) => cmdlet.WriteObject(value, enumerateCollection);
}