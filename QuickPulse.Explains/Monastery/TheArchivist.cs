using QuickPulse;
using QuickPulse.Explains.Abstractions;
using QuickPulse.Explains.Monastery.CodeLocator;
using QuickPulse.Explains.Monastery.Fragments;
using QuickPulse.Explains.Formatters;
using QuickPulse.Explains.Monastery.Writings;
using QuickPulse.Explains.Monastery.Reading;
using QuickPulse.Bolts;
using QuickPulse.Explains.Monastery.Fragments.Tables;


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
        var newLines =
            CodeReader.AsSnippet(GetCodeLocator().ReadAfter(docExample.Attribute.File, docExample.Attribute.Line))
                .Select(a => ApplyReplacements(a, docExample.Replacements))
                .Where(a => !string.IsNullOrWhiteSpace(a));
        var result = string.Join(Environment.NewLine, ApplyFormatters(newLines, docExample.Formatters));
        return new Example(docExample.Name, result);
    }

    private static Example ExampleFromCodeExample((string Name, CodeExampleAttribute Attribute, List<CodeReplaceAttribute> Replacements, List<CodeFormatAttribute> Formatters) docExample)
    {
        var newLines =
            CodeReader.AsExample(GetCodeLocator().ReadAfter(docExample.Attribute.File, docExample.Attribute.Line))
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
        return new(GetHeaderText(type), [.. nonLinks.Concat(links).Select(a => ToFragment(a, root, type))]);
    }

    public static Fragment ToFragment(DocFragmentAttribute attr, Type root, Type type) => attr switch
    {
        DocHeaderAttribute a => new HeaderFragment(a.Header, a.Level),
        DocContentAttribute a => new ContentFragment(a.Content),
        DocCodeAttribute a => new CodeFragment(a.Code, a.Language),
        DocIncludeAttribute a => new InclusionFragment(a.Included),
        DocExampleAttribute a => new CodeExampleFragment(a.Name, a.Language),
        DocCodeFileAttribute a => new CodeFragment(TheCartographer.GetFileContents(a.Path, a.Filename, a.SkipLines), a.Language),
        DocLinkAttribute a => new LinkFragment(a.Name, GetLinkLocation(type, a), GetLocalLinkLocation(a.Target)),
        DocTableAttribute a => new TableFragment(a.Columns, GetColumns(type, a)),
        _ => throw new NotSupportedException(attr.GetType().Name)
    };

    private static string GetLinkLocation(Type root, DocLinkAttribute a)
        => TheCartographer.ChartPath(root, a.Target)
            + (string.IsNullOrWhiteSpace(a.Section)
                ? "" : $"#{a.Section}").ToLower();
    private static string GetLocalLinkLocation(Type type)
        => "#" + FormatLink(GetHeaderText(type));

    private static string FormatLink(string input)
        => input
            .Replace(".", "")
            .Replace("<", "")
            .Replace(">", "")
            .Replace("&lt;", "")
            .Replace("&gt;", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace(' ', '-')
            .ToLower();

    // IEnumerables ?
    private static RowFragment[] GetColumns(Type type, DocTableAttribute attribute)
    {
        var typesWithColumns = TheReflectionist.GetColumns(type, attribute);
        var result = new List<RowFragment>();
        foreach (var (rowType, columns) in typesWithColumns)
        {
            FirstCellFragment firstCell = null!;
            var cells = new List<CellFragment>();
            var first = true;
            foreach (var col in attribute.Columns)
            {
                var column = columns.SingleOrDefault(a => a.ColumnName == col);
                var content = column == null ? " " : column.Content;
                if (first)
                {
                    first = false;
                    if (string.IsNullOrWhiteSpace(content))
                        content = GetHeaderText(rowType);
                    var path = TheCartographer.ChartPath(type, rowType);
                    firstCell = new(content, path, GetLocalLinkLocation(rowType));
                    continue;
                }
                cells.Add(new(content));
            }
            result.Add(new RowFragment(firstCell, [.. cells]));
        }
        return [.. result];
    }

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
