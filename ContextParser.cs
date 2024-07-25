namespace ConvertXliffToBcDevComments;

public static class ContextParser
{
    public static IEnumerable<ContextElement> Parse(string rawContext)
    {
        var elements = new List<ContextElement>();

        elements.AppendIfNotNull(TryConsumeElement(ref rawContext, @" \- Property ([\w\s]+)$", ContextElementType.Property));
        elements.PrependIfNotNull(TryConsumeElement(ref rawContext, @" \- Field ([\w\s]+)$", ContextElementType.Field));
        elements.PrependIfNotNull(TryConsumeElement(ref rawContext, @" \- Action ([\w\s]+)$", ContextElementType.Action));
        elements.PrependIfNotNull(TryConsumeElement(ref rawContext, @" \- EnumValue ([\w\s]+)$", ContextElementType.EnumValue));
        elements.PrependIfNotNull(TryConsumeElement(ref rawContext, @"^Table (.+)$", ContextElementType.Table));
        elements.PrependIfNotNull(TryConsumeElement(ref rawContext, @"^Page (.+)$", ContextElementType.Page));
        elements.PrependIfNotNull(TryConsumeElement(ref rawContext, @"^Enum (.+)$", ContextElementType.Enum));

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
            context = context.Remove(match.Index, match.Length);
            return elementBuilder.Invoke(match);
        }
        else
        {
            return null;
        }
    }
}