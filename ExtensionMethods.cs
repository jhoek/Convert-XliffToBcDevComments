using System.Collections;
using System.Management.Automation.Language;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.Dynamics.Nav.CodeAnalysis.Syntax;
using Microsoft.Dynamics.Nav.CodeAnalysis.Utilities;

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
                case RequestPageSyntax r:
                    yield return $"RequestPage RequestOptionsPage";
                    break;
                case var n when n.Kind.IsObject():
                    yield return $"{currentSyntaxNode.Kind.ToString().RegexReplace("Object$", "").RegexReplace("Type$", "")} {currentSyntaxNode.GetNameStringValue()}";
                    yield break;
                case var n when n.Kind == SyntaxKind.Field:
                    yield return $"Field {n.GetNameStringValue()}";
                    break;
                case var n when n.Kind == SyntaxKind.FieldGroup:
                    yield return $"FieldGroup {n.GetNameStringValue()}";
                    break;
                case FieldModificationSyntax f:
                    yield return $"Change {f.GetNameStringValue()}";
                    break;
                case ActionBaseSyntax a:
                    yield return $"Action {a.GetNameStringValue()}";
                    break;
                case ControlBaseSyntax c:
                    yield return $"Control {c.GetNameStringValue()}";
                    break;
                case PageViewSyntax v:
                    yield return $"View {v.GetNameStringValue()}";
                    break;
                case ReportDataItemSyntax d:
                    yield return $"ReportDataItem {d.GetNameStringValue()}";
                    break;
                case ReportColumnSyntax c:
                    yield return $"ReportColumn {c.GetNameStringValue()}";
                    break;
                case ReportLabelSyntax l:
                    yield return $"ReportLabel {l.GetNameStringValue()}";
                    break;
                case ReportLayoutSyntax l:
                    yield return $"ReportLayout {l.GetNameStringValue()}";
                    break;
                case QueryColumnSyntax c:
                    yield return $"QueryColumn {c.GetNameStringValue()}";
                    break;
                case XmlPortNodeSyntax n:
                    yield return $"XmlPortNode {n.GetNameStringValue()}";
                    break;
                case ControlModifyChangeSyntax m:
                    yield return $"Change {m.GetNameStringValue()}";
                    break;
                case ActionModifyChangeSyntax m:
                    yield return $"Change {m.GetNameStringValue()}";
                    break;
                case var n when n.Kind == SyntaxKind.EnumValue:
                    yield return $"EnumValue {n.GetNameStringValue()}";
                    break;
                case var n when n.Kind == SyntaxKind.Property:
                    yield return $"Property {n.As<PropertySyntax>().Name.Identifier.ValueText.UnquoteIdentifier()}";
                    break;
                case var n when n.Kind == SyntaxKind.VariableDeclaration:
                    yield return $"NamedType {n.GetNameStringValue()}";
                    break;
                case var n when n.Kind == SyntaxKind.MethodDeclaration:
                    yield return $"Method {n.GetNameStringValue()}";
                    break;
                case var n when n.Kind == SyntaxKind.TriggerDeclaration:
                    yield return $"Method {n.GetNameStringValue()}";
                    break;
            }

            switch (currentSyntaxNode)
            {
                case XmlPortNodeSyntax:
                    currentSyntaxNode = currentSyntaxNode.GetContainingApplicationObjectSyntax();
                    break;

                case ReportDataItemSyntax:
                    currentSyntaxNode = currentSyntaxNode.GetContainingApplicationObjectSyntax();
                    break;

                case ReportColumnSyntax:
                    currentSyntaxNode = currentSyntaxNode.GetContainingApplicationObjectSyntax();
                    break;

                case ControlBaseSyntax:
                    currentSyntaxNode = currentSyntaxNode.GetContainingApplicationObjectSyntax();
                    break;

                case ActionBaseSyntax:
                    currentSyntaxNode = currentSyntaxNode.GetContainingApplicationObjectSyntax();
                    break;

                default:
                    currentSyntaxNode = currentSyntaxNode.Parent;
                    break;
            }
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