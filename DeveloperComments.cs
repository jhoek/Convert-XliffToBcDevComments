using System.Collections;

namespace ConvertXliffToBcDevComments;

public class DeveloperComment
{
    public DeveloperComment(string languageCode, string value)
    {
        LanguageCode = languageCode;
        Value = value;
    }

    public DeveloperComment(string source)
    {
        if (source.Contains("="))
        {
            var elements = source.Split("=", 2, StringSplitOptions.TrimEntries);

            LanguageCode = elements[0];
            Value = elements[1];
        }
        else
        {
            Value = source;
        }
    }

    public string LanguageCode { get; }
    public string Value { get; }

    public override string ToString()
    {
        return LanguageCode is not null ? $"{LanguageCode}={Value}" : Value;
    }
}

public class DeveloperComments : IEnumerable<DeveloperComment>
{
    protected List<DeveloperComment> innerList = new List<DeveloperComment>();
    public string Separator { get; init; }

    public DeveloperComments(string source, string separator = "|")
    {
        Separator = separator;

        if (source is null)
            return;

        innerList
            .AddRange(
                (source)
                    .Split(separator)
                    .Select(c => new DeveloperComment(c))
            );
    }

    public bool ContainsLanguageCode(string languageCode) =>
        innerList.Any(c => (c.LanguageCode ?? "").Matches(languageCode));

    public string Get(string languageCode) =>
        this.Single(c => c.LanguageCode.Matches(languageCode)).Value;

    public IEnumerator<DeveloperComment> GetEnumerator() =>
        innerList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        innerList.GetEnumerator();

    public void Set(string languageCode, string value)
    {
        innerList.RemoveAll(c => c.LanguageCode.Matches(languageCode));
        innerList.Add(new DeveloperComment(languageCode, value));
    }

    public override string ToString() =>
        string.Join(Separator, innerList);
}