using System.Reflection;
using QuickPulse;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheReflectionist
{
    public static IOrderedEnumerable<Type> GetDocFileTypes(Type[] types) =>
        types.Where(a => a.GetCustomAttributes<DocFileAttribute>(false).Any())
            .OrderBy(t => t.Namespace)
            .ThenBy(t => t.Name);

    public static IEnumerable<Type> GetIncludedTypes(Type[] types) =>
        types.SelectMany(a => a.GetMethods().SelectMany(a => a.GetCustomAttributes<DocIncludeAttribute>(false)))
            .Select(a => a.Included)
            .Distinct();

    public static List<DocFragmentAttribute> GetDocFragmentAttributes(Type type) =>
        [.. type.GetCustomAttributes<DocFragmentAttribute>(false)
            .Concat(type.GetMethods()
                .SelectMany(a => a.GetCustomAttributes<DocFragmentAttribute>(false)))
            ];

    public static string? GetDocFileHeader(Type type) =>
        type.GetCustomAttribute<DocFileHeaderAttribute>(false)?.Header;
}
