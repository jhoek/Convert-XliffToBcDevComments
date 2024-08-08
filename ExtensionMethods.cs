using System.Runtime.CompilerServices;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ConvertXliffToBcDevComments;

public static class ExtensionMethods
{
    public static SyntaxNode Resolve(this IEnumerable<SyntaxNode> syntaxNodes, IEnumerable<ContextElement> contextElements)
    {
        switch (contextElements.First().Type)
        {
            case ContextElementType.Table:
                return syntaxNodes.OfType<TableSyntax>().Where(t => t.GetNameStringValue() == contextElements.First().Name).Resolve(contextElements.Skip(1));
            case ContextElementType.Field:
                return syntaxNodes.OfType<FieldSyntax>().Where(f => f.GetNameStringValue() == contextElements.First().Name).Resolve(contextElements.Skip(1));
            default:
                throw new ArgumentOutOfRangeException(nameof(contextElements), $"Don't know how to resolve a context element of type {contextElements.First().Type}");
        }
    }

    public static SyntaxNode Resolve(this SyntaxNode syntaxNode, IEnumerable<ContextElement> contextElements)
    {
        if (!contextElements.Any()) return syntaxNode;

        switch (contextElements.First().Type)
        {
            case ContextElementType.Field:
                return (syntaxNode as TableSyntax).Fields.Fields.Resolve(contextElements);
            default:
                throw new ArgumentOutOfRangeException(nameof(contextElements), $"Don't know how to resolve a context element of type {contextElements.First().Type}");
        }
    }

    public static void ApplyTo(this XliffTranslation xliffTranslation, IEnumerable<SyntaxNode> syntaxNodes) =>
        xliffTranslation
            .Context
            .ApplyTo(
                syntaxNodes,
                xliffTranslation.TargetLanguage,
                xliffTranslation.Target
            );

    public static void ApplyTo(this IEnumerable<ContextElement> contextElements, IEnumerable<SyntaxNode> syntaxNodes, string targetLanguage, string target)
    {
        var syntaxNode = syntaxNodes.FindFromContextElement(contextElements.First());

        if (contextElements.Count() == 2)
        {

        }


        // last element? apply
        // not last element? find syntaxNode and call recursively

    }

    public static SyntaxNode FindFromContextElement(this IEnumerable<SyntaxNode> syntaxNodes, ContextElement contextElement)
    {
        return syntaxNodes
            .Where(n => n.GetType() == contextElement.Type.ToSyntaxNodeType())
            .SingleOrDefault(n => n.GetNameStringValue() == contextElement.Name);
    }

    public static Type ToSyntaxNodeType(this ContextElementType contextElementType) =>
        contextElementType switch
        {
            ContextElementType.Table => typeof(TableSyntax),
            ContextElementType.Field => typeof(FieldSyntax),
            _ => throw new ArgumentOutOfRangeException(nameof(contextElementType))
        };

    // public static ObjectSyntax FindFromContext(this IEnumerable<ObjectSyntax> objects, IEnumerable<ContextElement> context, Action<string> writeWarning = null)
    // {
    //     var objectsOfType = context.First().Type switch
    //     {
    //         ContextElementType.Table => objects.OfType<TableSyntax>(),
    //         _ => throw new ArgumentOutOfRangeException()
    //     };

    //     var result = objectsOfType.SingleOrDefault(o => o.Name.Identifier.ValueText.Equals(context.First().Name));

    //     if (result is null) writeWarning?.Invoke($"{context.First().Type} {context.First().Name} could not be found.");

    //     return result;
    // }
}