using System.Collections;
using System.Management.Automation.Language;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;

namespace ConvertXliffToBcDevComments;

public static class ExtensionMethods
{
    public static T As<T>(this object value) =>
        (T)value;

    public static string ContextString(this SyntaxNode syntaxNode) =>
        syntaxNode
            .ContextStringElements()
            .Reverse()
            .JoinString(" - ");

    private static IEnumerable<string> ContextStringElements(this SyntaxNode syntaxNode)
    {
        var currentSyntaxNode = syntaxNode;

        while (true)
        {
            switch (currentSyntaxNode)
            {
                case var n when n.Kind.IsObject():
                    yield return $"{currentSyntaxNode.Kind.ToString().RegexReplace("Object$", "")} {currentSyntaxNode.GetNameStringValue()}";
                    yield break;
                case var n when n.Kind == SyntaxKind.Field:
                    yield return $"{currentSyntaxNode.Kind} {currentSyntaxNode.GetNameStringValue()}";
                    break;
                case var n when n.Kind == SyntaxKind.Property:
                    yield return $"{currentSyntaxNode.Kind} {currentSyntaxNode.As<PropertySyntax>().Name.Identifier.ValueText}";
                    break;
            }

            currentSyntaxNode = currentSyntaxNode.Parent;
        }
    }

    public static string RegexReplace(this string input, string pattern, string replacement) =>
        Regex.Replace(input, pattern, replacement);

    private static string JoinString(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

    public static bool Matches(this string value1, string value2) => value1.Equals(value2, StringComparison.InvariantCultureIgnoreCase);
    public static bool Matches(this IdentifierNameSyntax value1, string value2) => value1.Identifier.ValueText.Matches(value2);
    public static void WriteObject(this object value, Cmdlet cmdlet, bool enumerateCollection = true) => cmdlet.WriteObject(value, enumerateCollection);

    public static PropertyKind PropertyKind(this PropertySyntax propertySyntax) =>
        Enum.Parse<PropertyKind>(propertySyntax.Name.Identifier.ValueText, true);
}