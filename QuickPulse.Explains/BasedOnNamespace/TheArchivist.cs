
using QuickPulse;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheArchivist
{
    public static Book Compose<T>() => new(
        TheReflectionist.GetDocFileTypes(typeof(T).Assembly.GetTypes())
            .Select(a => PageFromType(typeof(T), a))
            .ToReadOnlyCollection(),
        TheReflectionist.GetIncludedTypes(typeof(T).Assembly.GetTypes())
            .Select(DocFileIncludedFromType)
            .ToReadOnlyCollection());

    private static Page PageFromType(Type root, Type type) => new(
        ExplanationFromType(type),
        ThePathfinder.NamespaceRelativeFilename(root, type));

    private static Explanation ExplanationFromType(Type type) =>
         new(GetHeaderText(type), TheReflectionist.GetDocMethods(type));

    private static Inclusion DocFileIncludedFromType(Type type) =>
        new(type, ExplanationFromType(type));

    private static string GetHeaderText(Type type)
    {
        var result = type.Name;
        if (result.Contains('_'))
            result = string.Join("", result.Split("_").Skip(1));
        result = result.Replace("_", " ");
        return result.Aggregate("", (a, b) => char.IsUpper(b) ? a + " " + b : a + b).Trim();
    }

    private static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        => source as IReadOnlyCollection<T> ?? source.ToList();
}
