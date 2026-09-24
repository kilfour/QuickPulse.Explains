using System.Reflection;
using QuickPulse;
using QuickPulse.Explains.Abstractions;
using QuickPulse.Explains.Formatters;

namespace QuickPulse.Explains.Monastery;

public static class TheReflectionist
{
    public static IOrderedEnumerable<Type> GetDocFileTypes(Type[] types) =>
        types.Where(a => a.GetCustomAttributes<DocFileAttribute>(false).Any())
            .OrderBy(t => t.Namespace)
            .ThenBy(t => t.Name);

    public static IEnumerable<(Type Type, bool NoHeader)> GetIncludedTypes(Type[] types) =>
        types.SelectMany(GetDocFragmentAttributes).OfType<DocIncludeAttribute>()
            .Select(a => (a.Included, a.NoHeader))
            .Distinct();

    public static List<DocFragmentAttribute> GetDocFragmentAttributes(Type type) =>
        [.. GetFragments(type)
            .Concat(type.GetMethods(Flags)
                .SelectMany(GetFragments))
            ];

    private static IEnumerable<DocFragmentAttribute> GetFragments(MemberInfo member) =>
        member.GetCustomAttributes(false).SelectMany(attribute => attribute switch
        {
            IExplainsAttribute composite => composite.Expand().OfType<DocFragmentAttribute>(),
            IDocAttribute composite => composite.Expand(),
            DocFragmentAttribute fragment => new[] { fragment },
            _ => Enumerable.Empty<DocFragmentAttribute>()
        });

    public static string? GetDocFileHeader(Type type) =>
        type.GetCustomAttribute<DocFileHeaderAttribute>(false)?.Header;

    public static IEnumerable<(string, CodeSnippetAttribute, List<CodeReplaceAttribute>, List<CodeFormatAttribute>)> GetDocSnippets(Type[] types) =>
        GetCodeMarkers<CodeSnippetAttribute>(GetCodeSources(types));

    public static IEnumerable<(string, CodeExampleAttribute, List<CodeReplaceAttribute>, List<CodeFormatAttribute>)> GetDocExamples(Type[] types) =>
        GetCodeMarkers<CodeExampleAttribute>(GetCodeSources(types));

    internal static List<(string Name, CodeAttribute[] Attributes)> GetCodeSources(Type[] types, HashSet<string>? referencedNames = null) =>
        types.SelectMany(type => new MemberInfo[] { type }
            .Concat(type.GetMethods(Flags))
            .Concat(type.GetFields(Flags)))
            .Select(member => (Member: member,
                Name: member is Type type ? type.FullName! : $"{member.DeclaringType!.FullName}.{member.Name}"))
            .Where(source => referencedNames is null || referencedNames.Contains(source.Name))
            .Select(source => (source.Name, GetCodeAttributes(source.Member)))
            .ToList();

    internal static IEnumerable<(string, T, List<CodeReplaceAttribute>, List<CodeFormatAttribute>)> GetCodeMarkers<T>(
        IEnumerable<(string Name, CodeAttribute[] Attributes)> sources) where T : CodeAttribute =>
        sources.SelectMany(source => source.Attributes.OfType<T>().Select(marker => (
            source.Name, marker,
            source.Attributes.OfType<CodeReplaceAttribute>().ToList(),
            source.Attributes.OfType<CodeFormatAttribute>().ToList())));

    private static CodeAttribute[] GetCodeAttributes(MemberInfo member)
    {
        var attributes = member.GetCustomAttributes(true);
        var names = attributes.Where(attribute => attribute is ICodeAttribute or IExplainsAttribute)
            .Select(attribute => attribute.GetType().Name).Distinct().ToArray();
        var expanded = attributes.SelectMany(attribute => attribute switch
        {
            IExplainsAttribute composite => composite.Expand().OfType<CodeAttribute>(),
            ICodeAttribute composite => composite.Expand(),
            CodeAttribute code => new[] { code },
            _ => Enumerable.Empty<CodeAttribute>()
        }).ToArray();
        foreach (var attribute in expanded)
            attribute.CompositionAttributeNames = names;
        return expanded;
    }

    public static IEnumerable<(Type Type, IEnumerable<DocColumnAttribute> Columns)> GetColumns(Type type, DocTableAttribute attribute)
        => type.Assembly.GetTypes()
            .Where(a => a.Namespace == type.Namespace + "." + attribute.NamespaceName)
            .Where(a => a.GetCustomAttributes<DocFileAttribute>(false).Any())
            .Select(t => (t, t.GetCustomAttributes<DocColumnAttribute>()));

    private readonly static BindingFlags Flags =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
}
