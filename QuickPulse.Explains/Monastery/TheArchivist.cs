using QuickPulse;
using QuickPulse.Arteries;
using QuickPulse.Explains.Abstractions;
using QuickPulse.Explains.Monastery.CodeLocator;
using QuickPulse.Explains.Monastery.Fragments;
using QuickPulse.Explains.Formatters;
using QuickPulse.Explains.Monastery.Writings;

namespace QuickPulse.Explains.Monastery;

public static class TheArchivist
{
    private static readonly AsyncLocal<Func<ICodeLocator>?> _override = new();

    public static Func<ICodeLocator> GetCodeLocator
    {
        get => _override.Value ?? (() => new FileSystemCodeLocator());
        set => _override.Value = value;
    }

    public static Book Compose<T>() => ComposeBook<T>(typeof(T).Assembly.GetTypes());
    public static Book ComposeOnly<T>() => ComposeBook<T>([typeof(T)]);

    private static Book ComposeBook<T>(Type[] types) => new(
        TheReflectionist.GetDocFileTypes(types)
            .Where(a => (a.Namespace ?? "").StartsWith(typeof(T).Namespace ?? ""))
            .Select(a => PageFromType(typeof(T), a))
            .ToReadOnlyCollection(),
        TheReflectionist.GetIncludedTypes(types)
            .Select(a => InclusionFromType(typeof(T), a))
            .ToReadOnlyCollection(),
        TheReflectionist.GetDocSnippets(typeof(T).Assembly.GetTypes())
            .Select(ExampleFromDocSnippet)
            .Concat(TheReflectionist.GetDocExamples(typeof(T).Assembly.GetTypes())
                .Select(ExampleFromCodeExample))
            .ToReadOnlyCollection());

    private static Example ExampleFromDocSnippet((string Name, CodeSnippetAttribute Attribute, List<CodeReplaceAttribute> Replacements, List<CodeFormatAttribute> Formatters) docExample)
    {
        var flow =
            from c in Pulse.Start<char>()
            from _ in Pulse.Prime(() => -1)
            from __ in Pulse.ManipulateIf<int>(c == '}', a => --a)
            from ___ in Pulse.TraceIf<int>(a => a >= 0, () => c)
            from ____ in Pulse.ManipulateIf<int>(c == '{', a => ++a)
            from s in Pulse.StopFlowingIf<int>(a => c == '}' && a < 0)
            select c;
        var text =
            Signal.From<string>(a => Pulse.ToFlow(flow, a))
                .SetArtery(Arteries.Text.Capture())
                .Pulse(GetCodeLocator().ReadAfter(docExample.Attribute.File, docExample.Attribute.Line))
                .GetArtery<StringSink>()
                .Content();
        var lines = text.Split(Environment.NewLine).Where(a => !string.IsNullOrWhiteSpace(a));
        var length = lines.Select(a => a.TakeWhile(a => a == ' ' || a == '\t').Count()).Min();
        var newLines = lines
            .Select(a => a.Substring(length))
            .Select(a => ApplyReplacements(a, docExample.Replacements))
            .Where(a => !string.IsNullOrWhiteSpace(a));
        var result = string.Join(Environment.NewLine, ApplyFormatters(newLines, docExample.Formatters));
        return new Example(docExample.Name, result);
    }

    public record FlowContext(int Brackets, int Braces, bool Printing);
    private static Example ExampleFromCodeExample((string Name, CodeExampleAttribute Attribute, List<CodeReplaceAttribute> Replacements, List<CodeFormatAttribute> Formatters) docExample)
    {
        var flow =
            from c in Pulse.Start<char>()
            from _ in Pulse.Prime(() => new FlowContext(-1, -1, false))
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
                .SetArtery(Arteries.Text.Capture())
                .Pulse(GetCodeLocator().ReadAfter(docExample.Attribute.File, docExample.Attribute.Line))
                .GetArtery<StringSink>()
                .Content();
        var lines = text.Split(Environment.NewLine).Where(a => !string.IsNullOrWhiteSpace(a));
        var length = lines.Count() > 1 ? lines.Skip(1).Select(a => a.TakeWhile(a => a == ' ' || a == '\t').Count()).Min() : 0;
        var newLines = new string[] { lines.First() }.Concat(lines.Skip(1)
            .Select(a => a.Substring(length)))
            .Select(a => ApplyReplacements(a, docExample.Replacements))
            .Where(a => !string.IsNullOrWhiteSpace(a));
        var result = string.Join(Environment.NewLine, ApplyFormatters(newLines, docExample.Formatters));
        return new Example(docExample.Name, result);
    }

    private static IEnumerable<string> ApplyFormatters(IEnumerable<string> code, List<CodeFormatAttribute> formatters)
    {
        var raw = code;
        foreach (var formatterAttr in formatters)
        {
            var formatter = (ICodeFormatter)Activator.CreateInstance(formatterAttr.FormatterType)!;
            raw = formatter.Format(raw);
        }
        return raw;
    }

    private static string ApplyReplacements(string code, List<CodeReplaceAttribute> replacements)
    {
        var raw = code;
        foreach (var repl in replacements)
        {
            raw = raw.Replace(repl.From, repl.To);
        }
        return raw;
    }

    private static Page PageFromType(Type root, Type type) => new(
        ExplanationFromType(root, type),
        TheCartographer.ChartPath(root, type));

    private static Explanation ExplanationFromType(Type root, Type type)
    {
        var fragments = TheReflectionist.GetDocFragmentAttributes(type).ToList();
        var nonLinks = fragments.Where(a => a is not DocLinkAttribute);
        var links = fragments.Where(a => a is DocLinkAttribute);
        return new(GetHeaderText(type), [.. nonLinks.Concat(links).Select(a => ToFragment(a, root))]);
    }

    public static Fragment ToFragment(DocFragmentAttribute attr, Type root) => attr switch
    {
        DocHeaderAttribute a => new HeaderFragment(a.Header, a.Level),
        DocContentAttribute a => new ContentFragment(a.Content),
        DocCodeAttribute a => new CodeFragment(a.Code, a.Language),
        DocIncludeAttribute a => new InclusionFragment(a.Included),
        DocExampleAttribute a => new CodeExampleFragment(a.Name, a.Language),
        DocCodeFileAttribute a => new CodeFragment(TheCartographer.GetFileContents(a.Path, a.Filename, a.SkipLines), a.Language),
        DocLinkAttribute a => new LinkFragment(a.Name, TheCartographer.ChartPath(root, a.Target) + (string.IsNullOrWhiteSpace(a.Section) ? "" : $"#{a.Section}")),
        _ => throw new NotSupportedException(attr.GetType().Name)
    };

    private static Inclusion InclusionFromType(Type root, Type type) =>
       new(type, ExplanationFromType(root, type));

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
