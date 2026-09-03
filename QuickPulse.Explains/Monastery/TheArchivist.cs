using QuickPulse;
using QuickPulse.Explains.Abstractions;
using QuickPulse.Explains.Monastery.CodeLocator;
using QuickPulse.Explains.Monastery.Fragments;
using QuickPulse.Explains.Formatters;
using QuickPulse.Explains.Monastery.Writings;
using QuickPulse.Explains.Monastery.Reading;
using QuickPulse.Explains.Monastery.Fragments.Tables;
using QuickPulse.Explains.Exceptions;
using System.Collections;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;


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

    private static Book ComposeBook<T>(Type[] types)
    {
        var root = typeof(T);
        var pageTypes = TheReflectionist.GetDocFileTypes(types)
            .Where(type => IsNamespaceInScope(root.Namespace, type.Namespace))
            .ToReadOnlyCollection();
        var pages = pageTypes
            .Select(type => PageFromType(root, type))
            .ToReadOnlyCollection();
        var inclusions = ResolveInclusions(root, pageTypes);
        var examples = ResolveExamples(
            pages.Select(page => page.Explanation)
                .Concat(inclusions.Select(inclusion => inclusion.Explanation)));

        return new(pages, inclusions, examples);
    }

    private static bool IsNamespaceInScope(string? rootNamespace, string? currentNamespace)
    {
        rootNamespace ??= "";
        currentNamespace ??= "";
        return rootNamespace.Length == 0
            || currentNamespace == rootNamespace
            || currentNamespace.StartsWith(rootNamespace + ".", StringComparison.Ordinal);
    }

    private static IReadOnlyCollection<Inclusion> ResolveInclusions(
        Type root,
        IReadOnlyCollection<Type> pageTypes)
    {
        var resolved = new Dictionary<Type, Inclusion>();
        var visiting = new List<Type>();

        foreach (var pageType in pageTypes)
            Visit(pageType);

        return [.. resolved.Values];

        void Visit(Type owner)
        {
            visiting.Add(owner);
            var includedTypes = TheReflectionist.GetIncludedTypes([owner])
                .Select(include => include.Type)
                .Distinct();

            foreach (var includedType in includedTypes)
            {
                var cycleStart = visiting.IndexOf(includedType);
                if (cycleStart >= 0)
                {
                    var cycle = visiting.Skip(cycleStart)
                        .Append(includedType)
                        .Select(type => type.FullName ?? type.Name);
                    throw new InvalidOperationException(
                        $"A documentation include cycle was found: {string.Join(" -> ", cycle)}.");
                }

                if (resolved.ContainsKey(includedType))
                    continue;

                resolved.Add(includedType, InclusionFromType(root, includedType, false));
                Visit(includedType);
            }

            visiting.RemoveAt(visiting.Count - 1);
        }
    }

    private static IReadOnlyCollection<Example> ResolveExamples(
        IEnumerable<Explanation> explanations)
    {
        var references = explanations
            .SelectMany(explanation => explanation.Fragments)
            .OfType<CodeExampleFragment>()
            .Where(reference => reference.SourceType is not null)
            .DistinctBy(reference => (reference.Name, reference.SourceType));
        var result = new List<Example>();

        foreach (var reference in references)
        {
            var sourceType = reference.SourceType!;
            var snippets = TheReflectionist.GetDocSnippets([sourceType])
                .Where(candidate => candidate.Item1 == reference.Name)
                .ToList();
            var examples = TheReflectionist.GetDocExamples([sourceType])
                .Where(candidate => candidate.Item1 == reference.Name)
                .ToList();

            if (snippets.Count + examples.Count > 1)
                throw new InvalidOperationException(
                    $"More than one code example matches '{reference.Name}'. " +
                    "Overloaded or multiply annotated members cannot be selected by name alone.");

            if (snippets.Count == 1)
                result.Add(ExampleFromDocSnippet(snippets[0]));
            else if (examples.Count == 1)
                result.Add(ExampleFromCodeExample(examples[0]));
        }

        return result;
    }

    private static Example ExampleFromDocSnippet((string Name, CodeSnippetAttribute Attribute, List<CodeReplaceAttribute> Replacements, List<CodeFormatAttribute> Formatters) docExample)
        => ExampleFrom(
                docExample.Name,
                docExample.Attribute.File,
                docExample.Attribute.Line,
                true,
                docExample.Replacements,
                docExample.Formatters);

    private static Example ExampleFromCodeExample(
        (string Name, CodeExampleAttribute Attribute, List<CodeReplaceAttribute> Replacements, List<CodeFormatAttribute> Formatters) docExample)
            => ExampleFrom(
                docExample.Name,
                docExample.Attribute.File,
                docExample.Attribute.Line,
                false,
                docExample.Replacements,
                docExample.Formatters);

    private static Example ExampleFrom(
        string name,
        string file,
        int line,
        bool asSnippet,
        List<CodeReplaceAttribute> replacements,
        List<CodeFormatAttribute> formatters)
    {
        var source = string.Join(
            Environment.NewLine,
            GetCodeLocator().ReadAfter(file, 0));
        var newLines =
            CodeExampleExtractor
                .ExtractSource(source, file, line, asSnippet)
                .ReplaceLineEndings()
                .Split(Environment.NewLine)
                .Select(a => ApplyReplacements(name, a, replacements));
        var formattedLines = ApplyFormatters(newLines, formatters).ToList();
        var indentCorrected = Dedent(formattedLines, asSnippet);
        var result = string.Join(Environment.NewLine, indentCorrected);
        return new Example(name, result);
    }

    public static IEnumerable<string> Dedent(IEnumerable<string> lines, bool asSnippet)
    {
        var materialized = lines.ToList();
        var first = materialized.FindIndex(line => !string.IsNullOrWhiteSpace(line));
        if (first < 0)
            return [];

        var last = materialized.FindLastIndex(line => !string.IsNullOrWhiteSpace(line));
        var content = materialized.GetRange(first, last - first + 1);
        var indent = content
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Min(line => line.TakeWhile(char.IsWhiteSpace).Count());
        return content.Select(line => DedentLine(line, indent));
    }

    private static string DedentLine(string line, int indent)
        => line.Length >= indent ? line[indent..] : line;

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

    private static string ApplyReplacements(string name, string code, List<CodeReplaceAttribute> replacements)
    {
        var raw = code;
        foreach (var repl in replacements)
        {
            if (string.IsNullOrEmpty(repl.From)) throw new EmptyStringUsedInCodeReplaceAttributeException(name);
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
        DocBarChartAttribute a => BarChartFrom(type, a),
        DocIncludeAttribute a => new InclusionFragment(a.Included, a.NoHeader),
        DocExampleAttribute a => new CodeExampleFragment(a.Name, a.Language, a.SourceType),
        DocCodeFileAttribute a => new CodeFragment(TheCartographer.GetFileContents(a.Path, a.Filename, a.SkipLines, a.NumberOfLines), a.Language),
        DocRawFileAttribute a => new ContentFragment(TheCartographer.GetRawFileContents(a.Path, a.Filename)),
        DocLinkAttribute a => new LinkFragment(a.Name, GetLinkLocation(type, a), GetLocalLinkLocation(a)),
        DocTableAttribute a => new TableFragment(a.Columns, GetColumns(type, a)),
        _ => throw new NotSupportedException(attr.GetType().Name)
    };

    private static BarChartFragment BarChartFrom(Type type, DocBarChartAttribute attribute)
    {
        var dataSourceType = attribute.DataSourceType ?? type;
        const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        var member = dataSourceType.GetField(attribute.DataMember, flags) as MemberInfo
            ?? dataSourceType.GetProperty(attribute.DataMember, flags);
        var data = member switch
        {
            FieldInfo field => field.GetValue(null),
            PropertyInfo property => property.GetValue(null),
            _ => throw new InvalidOperationException(
                $"Static chart data member '{attribute.DataMember}' was not found on '{dataSourceType.FullName}'.")
        };

        if (data is not IEnumerable points)
            throw new InvalidOperationException(
                $"Chart data member '{dataSourceType.FullName}.{attribute.DataMember}' must be an enumerable of (x, y) tuples.");

        var xValues = new List<string>();
        var yValues = new List<string>();
        foreach (var point in points)
        {
            if (point is not ITuple tuple || tuple.Length != 2)
                throw new InvalidOperationException(
                    $"Chart data member '{dataSourceType.FullName}.{attribute.DataMember}' must contain (x, y) tuples.");

            xValues.Add(FormatChartValue(tuple[0], dataSourceType, attribute));
            yValues.Add(FormatChartValue(tuple[1], dataSourceType, attribute));
        }

        return new(
            attribute.Title,
            attribute.XAxis,
            attribute.YAxis,
            xValues,
            yValues,
            attribute.YAxisMinimum,
            attribute.YAxisMaximum);
    }

    private static string FormatChartValue(object? value, Type type, DocBarChartAttribute attribute)
    {
        if (value is null || value is not IConvertible)
            throw new InvalidOperationException(
                $"Chart data member '{type.FullName}.{attribute.DataMember}' must contain numeric values.");

        try
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture)
                .ToString("G", CultureInfo.InvariantCulture);
        }
        catch (Exception exception) when (exception is FormatException or InvalidCastException or OverflowException)
        {
            throw new InvalidOperationException(
                $"Chart data member '{type.FullName}.{attribute.DataMember}' must contain numeric values.",
                exception);
        }
    }

    private static string GetLinkLocation(Type root, DocLinkAttribute attribute)
    {
        var path = TheCartographer.ChartLinkPath(root, attribute.Target);
        return string.IsNullOrWhiteSpace(attribute.Section)
            ? path
            : $"{path}#{FormatLink(attribute.Section)}";
    }

    private static string GetLocalLinkLocation(DocLinkAttribute attribute)
        => "#" + FormatLink(
            string.IsNullOrWhiteSpace(attribute.Section)
                ? GetHeaderText(attribute.Target)
                : attribute.Section);

    private static string GetLocalLinkLocation(Type type)
        => "#" + FormatLink(GetHeaderText(type));

    private static string FormatLink(string input)
    {
        var decoded = WebUtility.HtmlDecode(input);
        var result = new StringBuilder(decoded.Length);
        var previousWasWhitespace = false;

        foreach (var character in decoded)
        {
            if (char.IsWhiteSpace(character))
            {
                if (!previousWasWhitespace)
                    result.Append('-');
                previousWasWhitespace = true;
                continue;
            }

            previousWasWhitespace = false;
            if (char.IsLetterOrDigit(character) || character is '-' or '_')
                result.Append(char.ToLowerInvariant(character));
        }

        return result.ToString();
    }

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
                    var path = TheCartographer.ChartLinkPath(type, rowType);
                    firstCell = new(content, path, GetLocalLinkLocation(rowType));
                    continue;
                }
                cells.Add(new(content));
            }
            result.Add(new RowFragment(firstCell, [.. cells]));
        }
        return [.. result];
    }

    private static Inclusion InclusionFromType(Type root, Type type, bool noHeader) =>
       new(type, ExplanationFromType(root, type), noHeader);

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
        => source as IReadOnlyCollection<T> ?? [.. source];
}
