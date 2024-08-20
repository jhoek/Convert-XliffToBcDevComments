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
                case var n when n.Kind == SyntaxKind.FieldGroup:
                    yield return $"{currentSyntaxNode.Kind} {currentSyntaxNode.GetNameStringValue()}";
                    break;
                case ActionBaseSyntax a:
                    yield return $"Action {a.Name.Identifier.ValueText}";
                    break;
                case ControlBaseSyntax c:
                    yield return $"Control {c.Name.Identifier.ValueText}";
                    break;
                case PageViewSyntax v:
                    yield return $"View {v.Name.Identifier.ValueText}";
                    break;
                case ReportDataItemSyntax d:
                    yield return $"ReportDataItem {d.Name.Identifier.ValueText}";
                    break;
                case ReportColumnSyntax c:
                    yield return $"ReportColumn {c.Name.Identifier.ValueText}";
                    break;
                case ReportLabelSyntax l:
                    yield return $"ReportLabel {l.Name.Identifier.ValueText}";
                    break;
                case RequestPageSyntax r:
                    yield return $"RequestPage {r.Name.Identifier.ValueText}";
                    break;
                case QueryColumnSyntax c:
                    yield return $"QueryColumn {c.Name.Identifier.ValueText}";
                    break;
                case XmlPortNodeSyntax n:
                    yield return $"XmlPortNode {n.Name.Identifier.ValueText}";
                    break;
                case ControlModifyChangeSyntax m:
                    yield return $"Change {m.Name.Identifier.ValueText}";
                    break;
                case var n when n.Kind == SyntaxKind.EnumValue:
                    yield return $"EnumValue {n.As<EnumValueSyntax>().Name.Identifier.ValueText}";
                    break;
                case var n when n.Kind == SyntaxKind.Property:
                    yield return $"{currentSyntaxNode.Kind} {currentSyntaxNode.As<PropertySyntax>().Name.Identifier.ValueText}";
                    break;
                case var n when n.Kind == SyntaxKind.VariableDeclaration:
                    yield return $"NamedType {currentSyntaxNode.As<VariableDeclarationSyntax>().Name.Identifier.ValueText}";
                    break;
                case var n when n.Kind == SyntaxKind.MethodDeclaration:
                    yield return $"Method {currentSyntaxNode.As<MethodDeclarationSyntax>().Name.Identifier.ValueText}";
                    break;
            }

            /*
Change
EnumValue
            */

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