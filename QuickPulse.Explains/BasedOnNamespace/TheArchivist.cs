using System.Reflection;
using QuickPulse;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheArchivist
{
    public static Book Compose<T>() => new(
        GetDocFileTypes(typeof(T).Assembly.GetTypes())
            .Select(a => PageFromType(typeof(T), a))
            .ToReadOnlyCollection(),
        GetIncludedTypes(typeof(T).Assembly.GetTypes())
            .Select(DocFileIncludedFromType)
            .ToReadOnlyCollection());

    private static Page PageFromType(Type root, Type type) => new(
        ExplanationFromType(type),
        PathFromType(root, type));

    private static Explanation ExplanationFromType(Type type) =>
         new(GetHeaderText(type), GetDocMethods(type));

    private static Inclusion DocFileIncludedFromType(Type type) =>
        new(type, ExplanationFromType(type));

    private static string PathFromType(Type root, Type type) =>
        Path.Combine(GetNamespaceRelativePath(root, type), type.Name + ".md");

    private static IOrderedEnumerable<Type> GetDocFileTypes(Type[] types) =>
        types.Where(a => a.GetCustomAttributes<DocFileAttribute>(false).Any())
            .OrderBy(a => a.FullName);

    private static IEnumerable<Type> GetIncludedTypes(Type[] types) =>
        types.SelectMany(a => a.GetMethods().SelectMany(a => a.GetCustomAttributes<DocIncludeAttribute>(false)))
            .Select(a => a.IncludedDoc)
            .Distinct();

    private static List<DocMethodAttribute> GetDocMethods(Type type)
        => [.. type.GetMethods().SelectMany(a => a.GetCustomAttributes<DocMethodAttribute>(false))];

    private static string GetHeaderText(Type type)
    {
        var result = type.Name;
        if (result.Contains('_'))
            result = string.Join("", result.Split("_").Skip(1));
        result = result.Replace("_", " ");
        return result.Aggregate("", (a, b) => char.IsUpper(b) ? a + " " + b : a + b).Trim();
    }

    private static string GetNamespaceRelativePath(Type root, Type current)
    {
        var rootNs = root.Namespace ?? "";
        var currentNs = current.Namespace ?? "";

        string relativeNs = currentNs.StartsWith(rootNs)
            ? currentNs.Substring(rootNs.Length).TrimStart('.')
            : currentNs;

        return Path.Combine(relativeNs.Split('.', StringSplitOptions.RemoveEmptyEntries));
    }

    private static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        => source as IReadOnlyCollection<T> ?? source.ToList();
}
