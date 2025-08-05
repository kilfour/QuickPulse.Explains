
using QuickPulse;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheArchivist
{
    public static Book Compose<T>() => new(
        TheReflectionist.GetDocFileTypes(typeof(T).Assembly.GetTypes())
            .Where(a => (a.Namespace ?? "").StartsWith(typeof(T).Namespace ?? ""))
            .Select(a => PageFromType(typeof(T), a))
            .ToReadOnlyCollection(),
        TheReflectionist.GetIncludedTypes(typeof(T).Assembly.GetTypes())
            .Select(InclusionFromType)
            .ToReadOnlyCollection(),
        TheReflectionist.GetDocExamples(typeof(T).Assembly.GetTypes())
            .Select(ExampleFromType)
            .ToReadOnlyCollection());

    private static Example ExampleFromType((string Name, DocExampleAttribute Attribute) docExample)
    {
        var holden = TheString.Catcher();
        var signal =
            Signal.From<string>(
                a =>
                    from cnt in Pulse.Gather(-1)
                    from _ in Pulse.ToFlow(
                        c =>
                            from _ in Pulse.ManipulateIf<int>(c == '}', a => a - 1)
                            from __ in Pulse.TraceIf<int>(a => a >= 0, () => c)
                            from ___ in Pulse.ManipulateIf<int>(c == '{', a => a + 1)
                            select Unit.Instance, a)
                    select Unit.Instance)
            .SetArtery(holden);
        foreach (var line in File.ReadLines(docExample.Attribute.File).Skip(docExample.Attribute.Line))
        {
            signal.Pulse(line);
            signal.Pulse(Environment.NewLine);
        }
        var text = holden.Whispers().Split(Environment.NewLine).Where(a => !string.IsNullOrWhiteSpace(a));
        var length = text.Select(a => a.TakeWhile(a => a == ' ').Count()).Min();
        var result = string.Join(Environment.NewLine, text.Select(a => a.Substring(length)));
        return new Example(docExample.Name, result);
    }

    private static Page PageFromType(Type root, Type type) => new(
        ExplanationFromType(type),
        TheCartographer.ChartPath(root, type));

    private static Explanation ExplanationFromType(Type type) =>
         new(GetHeaderText(type), [.. TheReflectionist.GetDocFragmentAttributes(type).Select(ToFragment)]);

    public static Fragment ToFragment(this DocFragmentAttribute attr) => attr switch
    {
        DocHeaderAttribute a => new HeaderFragment(a.Header, a.Level),
        DocContentAttribute a => new ContentFragment(a.Content),
        DocCodeAttribute a => new CodeFragment(a.Code, a.Language),
        DocIncludeAttribute a => new InclusionFragment(a.Included),
        DocCodeExampleAttribute a => new CodeExampleFragment(a.Name, a.Replacements),
        _ => throw new NotSupportedException(attr.GetType().Name)
    };

    private static Inclusion InclusionFromType(Type type) =>
       new(type, ExplanationFromType(type));

    private static string GetHeaderText(Type type)
    {
        var docFileHeader = TheReflectionist.GetDocFileHeader(type);
        if (!string.IsNullOrEmpty(docFileHeader)) return docFileHeader;
        var result = type.Name;
        result = result.EndsWith("Tests") ? result.Substring(0, result.Length - 5) : result;
        result = result.EndsWith("Test") ? result.Substring(0, result.Length - 4) : result;
        if (result.Contains('_'))
            result = string.Join("", result.Split("_").Skip(1));
        result = result.Replace("_", " ");
        return result.Aggregate("", (a, b) => char.IsUpper(b) ? a + " " + b : a + b).Trim();
    }

    public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        => source as IReadOnlyCollection<T> ?? source.ToList();
}
