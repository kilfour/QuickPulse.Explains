using QuickPulse;
using QuickPulse.Arteries;
using QuickPulse.Explains.Abstractions;
using QuickPulse.Explains.BasedOnNamespace.Fragments;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheArchivist
{
    public static Book Compose<T>() => ComposeBook<T>(typeof(T).Assembly.GetTypes());
    public static Book ComposeOnly<T>() => ComposeBook<T>([typeof(T)]);

    private static Book ComposeBook<T>(Type[] types) => new(
        TheReflectionist.GetDocFileTypes(types)
            .Where(a => (a.Namespace ?? "").StartsWith(typeof(T).Namespace ?? ""))
            .Select(a => PageFromType(typeof(T), a))
            .ToReadOnlyCollection(),
        TheReflectionist.GetIncludedTypes(types)
            .Select(InclusionFromType)
            .ToReadOnlyCollection(),
        TheReflectionist.GetDocSnippets(typeof(T).Assembly.GetTypes())
            .Select(ExampleFromDocSnippet)
            .Concat(TheReflectionist.GetDocExamples(typeof(T).Assembly.GetTypes())
                .Select(ExampleFromDocExample))
            .ToReadOnlyCollection());

    private static Example ExampleFromDocSnippet((string Name, DocSnippetAttribute Attribute, List<DocReplaceAttribute> Replacements) docExample)
    {
        var flow =
            from c in Pulse.Start<char>()
            from _ in Pulse.Gather(-1)
            from __ in Pulse.ManipulateIf<int>(c == '}', a => --a)
            from ___ in Pulse.TraceIf<int>(a => a >= 0, () => c)
            from ____ in Pulse.ManipulateIf<int>(c == '{', a => ++a)
            from s in Pulse.StopFlowingIf<int>(a => c == '}' && a < 0)
            select c;
        var text =
            Signal.From<string>(a => Pulse.ToFlow(flow, a))
                .SetArtery(TheString.Catcher())
                .Pulse(File.ReadLines(docExample.Attribute.File)
                    .Skip(docExample.Attribute.Line)
                    .Select(a => a + Environment.NewLine))
                .GetArtery<Holden>()
                .Whispers();
        var lines = text.Split(Environment.NewLine).Where(a => !string.IsNullOrWhiteSpace(a));
        var length = lines.Select(a => a.TakeWhile(a => a == ' ' || a == '\t').Count()).Min();
        var result = string.Join(Environment.NewLine, lines
            .Select(a => a.Substring(length))
            .Select(a => ApplyReplacements(a, docExample.Replacements))
            .Where(a => !string.IsNullOrWhiteSpace(a)));
        return new Example(docExample.Name, result);
    }

    public record FlowContext(int Brackets, int Braces, bool Printing);
    private static Example ExampleFromDocExample((string Name, DocExampleAttribute Attribute, List<DocReplaceAttribute> Replacements) docExample)
    {
        var flow =
            from c in Pulse.Start<char>()
            from _ in Pulse.Gather(new FlowContext(-1, -1, false))
            from a___ in Pulse.ManipulateIf<FlowContext>(c == '[', a => a with { Brackets = a.Brackets + 1 })
            from a__ in Pulse.ManipulateIf<FlowContext>(a => !char.IsWhiteSpace(c) && a.Brackets < 0, a => a with { Printing = true })
            from a_ in Pulse.ManipulateIf<FlowContext>(c == ']', a => a with { Brackets = a.Brackets - 1 })
            from __ in Pulse.ManipulateIf<FlowContext>(c == '}', a => a with { Braces = a.Braces - 1 })
            from ___ in Pulse.TraceIf<FlowContext>(a => a.Printing, () => c)
            from ____ in Pulse.ManipulateIf<FlowContext>(c == '{', a => a with { Braces = a.Braces + 1 })
            from s in Pulse.StopFlowingIf<FlowContext>(a => c == '}' && a.Braces < 0)
            select c;
        var text =
            Signal.From<string>(a => Pulse.ToFlow(flow, a))
                .SetArtery(TheString.Catcher())
                .Pulse(File.ReadLines(docExample.Attribute.File)
                    .Skip(docExample.Attribute.Line)
                    .Select(a => a + Environment.NewLine))
                .GetArtery<Holden>()
                .Whispers();
        var lines = text.Split(Environment.NewLine).Where(a => !string.IsNullOrWhiteSpace(a));
        var length = lines.Count() > 1 ? lines.Skip(1).Select(a => a.TakeWhile(a => a == ' ' || a == '\t').Count()).Min() : 0;
        var result = string.Join(Environment.NewLine, new string[] { lines.First() }.Concat(lines.Skip(1)
            .Select(a => a.Substring(length)))
            .Select(a => ApplyReplacements(a, docExample.Replacements))
            .Where(a => !string.IsNullOrWhiteSpace(a)));
        return new Example(docExample.Name, result);
    }

    private static string ApplyReplacements(string code, List<DocReplaceAttribute> replacements)
    {
        var raw = code;
        foreach (var repl in replacements)
        {
            raw = raw.Replace(repl.From, repl.To);
        }
        return raw;
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
        DocCodeExampleAttribute a => new CodeExampleFragment(a.Name),
        DocCodeFileAttribute a => new CodeFragment(TheCartographer.GetFileContents(a.Path, a.Filename), a.Language),
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
