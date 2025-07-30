using System.Reflection;
using QuickPulse;

namespace QuickPulse.Explains.BasedOnNamespace;

public class Reflectionist
{
    public IOrderedEnumerable<Type> GetDocFileTypes(Type[] types) =>
        types.Where(a => a.GetCustomAttributes<DocFileAttribute>(false).Any())
            .OrderBy(t => t.Namespace)
            .ThenBy(t => t.Name);

    public IEnumerable<Type> GetIncludedTypes(Type[] types) =>
        types.SelectMany(a => a.GetMethods().SelectMany(a => a.GetCustomAttributes<DocIncludeAttribute>(false)))
            .Select(a => a.Included)
            .Distinct();

    public List<DocFragmentAttribute> GetDocFragmentAttributes(Type type) =>
        [.. type.GetCustomAttributes<DocFragmentAttribute>(false)
            .Concat(type.GetMethods()
                .SelectMany(a => a.GetCustomAttributes<DocFragmentAttribute>(false)))
            ];

    public string? GetDocFileHeader(Type type) =>
        type.GetCustomAttribute<DocFileHeaderAttribute>(false)?.Header;
}
