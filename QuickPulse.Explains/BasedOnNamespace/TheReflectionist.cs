using System.Reflection;
using QuickPulse;
using QuickPulse.Explains.Abstractions;
using QuickPulse.Explains.Formatters;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheReflectionist
{
    public static IOrderedEnumerable<Type> GetDocFileTypes(Type[] types) =>
        types.Where(a => a.GetCustomAttributes<DocFileAttribute>(false).Any())
            .OrderBy(t => t.Namespace)
            .ThenBy(t => t.Name);

    public static IEnumerable<Type> GetIncludedTypes(Type[] types) =>
        types.SelectMany(a => a.GetCustomAttributes<DocIncludeAttribute>(false).Select(a => a.Included))
            .Concat(types.SelectMany(a => a.GetMethods(Flags).SelectMany(a => a.GetCustomAttributes<DocIncludeAttribute>(false)))
            .Select(a => a.Included))
            .Distinct();

    public static List<DocFragmentAttribute> GetDocFragmentAttributes(Type type) =>
        [.. type.GetCustomAttributes<DocFragmentAttribute>(false)
            .Concat(type.GetMethods(Flags)
                .SelectMany(a => a.GetCustomAttributes<DocFragmentAttribute>(false)))
            ];

    public static string? GetDocFileHeader(Type type) =>
        type.GetCustomAttribute<DocFileHeaderAttribute>(false)?.Header;

    public static IEnumerable<(string, CodeSnippetAttribute, List<CodeReplaceAttribute>, List<CodeFormatAttribute>)> GetDocSnippets(Type[] types) =>
        types.SelectMany(a => a.GetMethods(Flags))
            .Where(a => a.GetCustomAttributes<CodeSnippetAttribute>().Any())
            .Select(a => (
                $"{a.DeclaringType!.FullName}.{a.Name}",
                a.GetCustomAttribute<CodeSnippetAttribute>(),
                a.GetCustomAttributes<CodeReplaceAttribute>().ToList(),
                a.GetCustomAttributes<CodeFormatAttribute>().ToList()))!;

    public static IEnumerable<(string, CodeExampleAttribute, List<CodeReplaceAttribute>, List<CodeFormatAttribute>)> GetDocExamples(Type[] types) =>
        types.Where(a => a.GetCustomAttributes<CodeExampleAttribute>().Any())
            .Select(a => (
                a.FullName!,
                a.GetCustomAttribute<CodeExampleAttribute>()!,
                a.GetCustomAttributes<CodeReplaceAttribute>().ToList(),
                a.GetCustomAttributes<CodeFormatAttribute>().ToList())).Concat(
        types.SelectMany(a => a.GetMethods(Flags))
            .Where(a => a.GetCustomAttributes<CodeExampleAttribute>().Any())
            .Select(a => (
                $"{a.DeclaringType!.FullName!}.{a.Name}",
                a.GetCustomAttribute<CodeExampleAttribute>()!,
                a.GetCustomAttributes<CodeReplaceAttribute>().ToList(),
                a.GetCustomAttributes<CodeFormatAttribute>().ToList())));

    private readonly static BindingFlags Flags =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
}