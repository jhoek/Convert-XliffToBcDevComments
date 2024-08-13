using System.Collections;

namespace ConvertXliffToBcDevComments;

public class DeveloperComment
{
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

    public string LanguageCode { get; init; }
    public required string Value { get; init; }

    public override string ToString()
    {
        return LanguageCode is not null ? $"{LanguageCode}={Value}" : Value;
    }
}

public class DeveloperComments : IEnumerable<DeveloperComment>
{
    protected List<DeveloperComment> innerList = new List<DeveloperComment>();

    public DeveloperComments(string source, string separator = "|")
    {
        innerList.AddRange(source.Split(separator));
    }

    public string Get(string languageCode) =>
        this
            .Single(c => c.StartsWith(languageCode))
            .Split("=")
            .Skip(1)
            .FirstOrDefault();

    public IEnumerator<DeveloperComment> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public string Set(string languageCode) { }

    public override string ToString()
    {
        return base.ToString();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}