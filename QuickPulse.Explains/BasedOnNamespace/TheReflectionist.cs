
using System.Reflection;
using QuickPulse;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheReflectionist
{
    public static IOrderedEnumerable<Type> GetDocFileTypes(Type[] types) =>
        types.Where(a => a.GetCustomAttributes<DocFileAttribute>(false).Any())
            .OrderBy(a => a.FullName);

    public static IEnumerable<Type> GetIncludedTypes(Type[] types) =>
        types.SelectMany(a => a.GetMethods().SelectMany(a => a.GetCustomAttributes<DocIncludeAttribute>(false)))
            .Select(a => a.IncludedDoc)
            .Distinct();

    public static List<DocMethodAttribute> GetDocMethods(Type type)
        => [.. type.GetMethods().SelectMany(a => a.GetCustomAttributes<DocMethodAttribute>(false))];
}
