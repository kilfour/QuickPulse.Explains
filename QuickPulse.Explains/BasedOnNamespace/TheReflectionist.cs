
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

    public static List<Fragment> GetDocMethods(Type type) =>
        [.. type.GetCustomAttributes<DocFragmentAttribute>(false)
            .Concat(type.GetMethods()
                .SelectMany(a => a.GetCustomAttributes<DocFragmentAttribute>(false)))
            .Select(a => a.ToFragment())];

    public static string? GetDocFileHeader(Type type) =>
        type.GetCustomAttribute<DocFileHeaderAttribute>(false)?.Header;

    public static Fragment ToFragment(this DocFragmentAttribute attr) => attr switch
    {
        DocHeaderAttribute h => new HeaderFragment(h.Header, h.Level),
        DocContentAttribute c => new ContentFragment(c.Content),
        DocIncludeAttribute i => new InclusionFragment(i.Included),
        _ => throw new NotSupportedException(attr.GetType().Name)
    };
}
