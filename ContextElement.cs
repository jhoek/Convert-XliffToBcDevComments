using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Management.Automation.Provider;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ConvertXliffToBcDevComments;

public enum ContextElementType
{
    None,
    Table,
    Field,
    Enum,
    EnumValue,
    Property,
    NamedType
}

public class ContextElement
{
    public ContextElement(ContextElementType type, string name)
    {
        Type = type;
        Name = name;
    }

    public ContextElementType Type { get; init; }
    public string Name { get; init; }
}

public static class ContextParser
{
    public static IEnumerable<ContextElement> Parse(string rawContext)
    {
        var elements = new List<ContextElement>();

        elements.AppendIfNotNull(TryConsumeElement(ref rawContext, @" - Property ({\w+})$", ContextElementType.Property));
        elements.PrependIfNotNull(TryConsumeElement(ref rawContext, @"- Field ({\w+})$", ContextElementType.Field));
        elements.PrependIfNotNull(TryConsumeElement(ref rawContext, @"^Table ({\w+})$", ContextElementType.Table));

        return elements;
    }

    public static void AppendIfNotNull<T>(this List<T> list, T item)
    {
        if (item is not null) list.Add(item);
    }

    public static void PrependIfNotNull<T>(this List<T> list, T item)
    {
        if (item is not null) list.Insert(0, item);
    }

    public static ContextElement TryConsumeElement(ref string context, string pattern, ContextElementType type) =>
        TryConsumeElement(ref context, pattern, m => new ContextElement(type, m.Groups[1].Value));

    public static ContextElement TryConsumeElement(ref string context, string pattern, Func<Match, ContextElement> elementBuilder)
    {
        var match = Regex.Match(context, pattern);

        if (match.Success)
        {
            context = context.Substring(match.Index);
            return elementBuilder.Invoke(match);
        }
        else
        {
            return null;
        }
    }
}