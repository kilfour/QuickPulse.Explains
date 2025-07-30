
using QuickPulse;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheArchivist
{
    public static Book Compose<T>() => new(
        TheReflectionist.GetDocFileTypes(typeof(T).Assembly.GetTypes())
            .Select(a => PageFromType(typeof(T), a))
            .ToReadOnlyCollection(),
        TheReflectionist.GetIncludedTypes(typeof(T).Assembly.GetTypes())
            .Select(InclusionFromType)
            .ToReadOnlyCollection());

    private static Page PageFromType(Type root, Type type) => new(
        ExplanationFromType(type),
        TheCartographer.ChartPath(root, type));

    private static Explanation ExplanationFromType(Type type) =>
         new(GetHeaderText(type), [.. TheReflectionist.GetDocFragmentAttributes(type).Select(ToFragment)]);

    public static Fragment ToFragment(this DocFragmentAttribute attr) => attr switch
    {
        DocHeaderAttribute h => new HeaderFragment(h.Header, h.Level),
        DocContentAttribute c => new ContentFragment(c.Content),
        DocIncludeAttribute i => new InclusionFragment(i.Included),
        _ => throw new NotSupportedException(attr.GetType().Name)
    };

    private static Inclusion InclusionFromType(Type type) =>
       new(type, ExplanationFromType(type));

    private static string GetHeaderText(Type type)
    {
        var docFileHeader = TheReflectionist.GetDocFileHeader(type);
        if (!string.IsNullOrEmpty(docFileHeader)) return docFileHeader;
        var result = type.Name;
        if (result.Contains('_'))
            result = string.Join("", result.Split("_").Skip(1));
        result = result.Replace("_", " ");
        return result.Aggregate("", (a, b) => char.IsUpper(b) ? a + " " + b : a + b).Trim();
    }

    public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        => source as IReadOnlyCollection<T> ?? source.ToList();
}
