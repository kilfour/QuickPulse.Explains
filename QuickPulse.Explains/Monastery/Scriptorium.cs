using QuickPulse.Explains.Exceptions;
using QuickPulse.Explains.Monastery.Fragments;
using QuickPulse.Explains.Monastery.Fragments.Tables;
using QuickPulse.Explains.Monastery.Writings;
using System.Globalization;
using System.Text;

namespace QuickPulse.Explains.Monastery;

public static class Scriptorium
{
    private const char Separator = '/';

    public static Flow<Flow> LoadReference(Reference reference) =>
        from includes in Pulse.Prime(() => reference.Inclusions)
        from examples in Pulse.Prime(() => reference.Examples)
        from level in Pulse.Prime(() => 1)
        select Flow.Continue;

    private static Flow<Flow> MarkDownHeader(string text) =>
        from level in Pulse.Draw<int>()
        let headingMarker = new string('#', level)
        let header = $"{headingMarker} {text}"
        from _ in Pulse.Trace(header)
        select Flow.Continue;

    private static Flow<Flow> Header(HeaderFragment fragment) =>
        from level in Pulse.Draw<int>()
        let headingMarker = new string('#', level + fragment.Level)
        from _ in Pulse.Trace($"{headingMarker} {fragment.Header}")
        select Flow.Continue;

    private static Flow<Flow> Content(ContentFragment fragment) =>
        Pulse.Trace($"{fragment.Content}  ");

    private static Flow<Flow> Code(CodeFragment fragment) =>
         from s in Pulse.Trace($"```{fragment.Language}")
         from _ in Pulse.Trace(fragment.Code.Trim())
         from e in Pulse.Trace("```")
         select Flow.Continue;

    private static Flow<Flow> BarChart(BarChartFragment fragment) =>
        from start in Pulse.Trace("```mermaid")
        from chart in Pulse.Trace("xychart-beta")
        from title in Pulse.Trace($"    title \"{EscapeMermaidLabel(fragment.Title)}\"")
        from xAxis in Pulse.Trace($"    x-axis \"{EscapeMermaidLabel(fragment.XAxis)}\" [{string.Join(", ", fragment.XValues)}]")
        from yAxis in Pulse.Trace(YAxis(fragment))
        from bars in Pulse.Trace($"    bar [{string.Join(", ", fragment.YValues)}]")
        from end in Pulse.Trace("```")
        select Flow.Continue;

    private static string YAxis(BarChartFragment fragment) =>
        fragment.YAxisMinimum is double minimum && fragment.YAxisMaximum is double maximum
            ? $"    y-axis \"{EscapeMermaidLabel(fragment.YAxis)}\" {FormatNumber(minimum)} --> {FormatNumber(maximum)}"
            : $"    y-axis \"{EscapeMermaidLabel(fragment.YAxis)}\"";

    private static string EscapeMermaidLabel(string value)
    {
        var normalized = value.ReplaceLineEndings(" ");
        var result = new StringBuilder(normalized.Length);
        foreach (var character in normalized)
        {
            result.Append(character switch
            {
                '#' => "#35;",
                '&' => "#38;",
                '"' => "#34;",
                '<' => "#60;",
                '>' => "#62;",
                _ when char.IsControl(character) => " ",
                _ => character
            });
        }
        return result.ToString();
    }

    private static string FormatNumber(double value) =>
        value.ToString("G", CultureInfo.InvariantCulture);

    private static Flow<Flow> CodeExample(CodeExampleFragment fragment) =>
         from examples in Pulse.Draw<IReadOnlyCollection<Example>>()
         let example = examples.SingleOrDefault(a => a.Name == fragment.Name)
         from s in Pulse.Trace($"```{fragment.Language}")
         from _ in Pulse.TraceIf(example != null, () => example.Code)
         from check in Pulse.When(example == null, () => throw new CodeExampleNotFoundException(fragment.Name))
         from e in Pulse.Trace("```")
         select Flow.Continue;

    private static Flow<Flow> Include(InclusionFragment fragment) =>
        from includes in Pulse.Draw<IReadOnlyCollection<Inclusion>>()
        let include = includes.Single(a => a.Type == fragment.Included)
        let noHeader = fragment.NoHeader || include.NoHeader
        from _1 in Pulse.ToFlowIf(noHeader, Fragments!, () => include.Explanation.Fragments)
        from _2 in Pulse.ToFlowIf(!noHeader, Explanation!, () => include.Explanation)
        select Flow.Continue;

    private static Flow<Flow> Link(LinkFragment fragment) =>
        from includeLinks in Pulse.Draw<bool>()
        from includes in Pulse.Draw<IReadOnlyCollection<Inclusion>>()
        from newLine in Pulse.Trace("")
        let link = includeLinks
           ? fragment.Link
           : fragment.LocalLink
        from _2 in Pulse.Trace($"[{fragment.Name}]: {link}")
        select Flow.Continue;


    private static Flow<Flow> TableRow(IEnumerable<string> row) =>
        Pulse.Trace($"| {string.Join("| ", row.Select(EscapeMarkdownTableCell))} |");

    private static string EscapeMarkdownTableCell(string value) =>
        value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("|", "\\|", StringComparison.Ordinal)
            .ReplaceLineEndings("<br>");

    private static Flow<Flow> Row(RowFragment fragment) =>
        from includeLinks in Pulse.Draw<bool>()
        let link = includeLinks
            ? fragment.FirstCell.Link
            : fragment.FirstCell.LocalLink
        let firstCell = $"[{fragment.FirstCell.Content}]({link})"
        let cells = fragment.Cells.Select(a => a.Content).Prepend(firstCell)
        from row in Pulse.ToFlow(TableRow, cells)
        select Flow.Continue;

    private static Flow<Flow> Table(TableFragment fragment) =>
        from headers in Pulse.ToFlow(TableRow, fragment.Headers)
        from divider in Pulse.ToFlow(TableRow, fragment.Headers.Select(_ => "-").ToArray())
        from body in Pulse.ToFlow(Row, fragment.Body)
        select Flow.Continue;

    private static Flow<Flow> Fragment(Fragment fragment) =>
        from _ in fragment switch
        {
            HeaderFragment a => Pulse.ToFlow(b => Pulse.ToFlow(Header, b), a),
            ContentFragment a => Pulse.ToFlow(Content, a),
            CodeFragment a => Pulse.ToFlow(Code, a),
            BarChartFragment a => Pulse.ToFlow(BarChart, a),
            CodeExampleFragment a => Pulse.ToFlow(CodeExample, a),
            InclusionFragment a => Pulse.ToFlow(Include, a),
            LinkFragment a => Pulse.ToFlow(Link, a),
            TableFragment a => Pulse.ToFlow(Table, a),
            _ => Pulse.NoOp()
        }
        select Flow.Continue;

    private static Flow<Flow> Fragments(IEnumerable<Fragment> fragments) =>
        Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Fragment, fragments));

    private static Flow<Flow> Explanation(Explanation explanation) =>
        from _1 in Pulse.ToFlow(MarkDownHeader, explanation.HeaderText)
        from _2 in Pulse.ToFlow(Fragments, explanation.Fragments)
        select Flow.Continue;

    private static Flow<Flow> BookPage(Page page) =>
        Pulse.Scoped<int>(a => page.Path.Split(Separator).Length, Pulse.ToFlow(Explanation, page.Explanation));

    // this is the entrypoint for single page doc
    public static Flow<Flow> Book(Book book) =>
        from excludeLinks in Pulse.Prime(() => false)
        from initialize in Pulse.ToFlow(LoadReference, book)
        from _ in Pulse.ToFlow(BookPage, book.Pages)
        select Flow.Continue;

    // this is the entry point for writing a doc folder
    public static Flow<Flow> SinglePage(SinglePage pageAndReference) =>
        from includeLinks in Pulse.Prime(() => true)
        from initialize in Pulse.ToFlow(LoadReference, pageAndReference)
        from _ in Pulse.Scoped<int>(a => 1, Pulse.ToFlow(Explanation, pageAndReference.Page.Explanation))
        select Flow.Continue;

    public static Flow<Flow> TableOfContent(Chronicle chronicle) =>
        from _ in Pulse.NoOp()
        let level = chronicle.Path.Split(Separator).Length - 1
        let indent = new string(' ', level * 2)
        from trace in Pulse.Trace($"{indent}- [{chronicle.Text}]({chronicle.Path})")
        select Flow.Continue;
}

