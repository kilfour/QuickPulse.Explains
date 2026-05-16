using QuickPulse.Explains.Exceptions;
using QuickPulse.Explains.Monastery.Fragments;
using QuickPulse.Explains.Monastery.Fragments.Tables;
using QuickPulse.Explains.Monastery.Writings;

namespace QuickPulse.Explains.Monastery;

public static class Scriptorium
{
    private const char Separator = '/';

    public static Flow<Flow> LoadReference(Reference reference) =>
        Pulse
            .Prime(() => reference.Inclusions).Dissipate()
            .Prime(() => reference.Examples).Dissipate()
            .Prime(() => 1).Dissipate();

    private static Flow<Flow> MarkDownHeader(string text) =>
        from level in Pulse.Draw<int>()
        let headingMarker = new string('#', level)
        let header = $"{headingMarker} {text}"
        from _ in Pulse.Trace(header)
        select Flow.Continue;

    private static Flow<Flow> Header(HeaderFragment fragment) =>
        Pulse
            .Draw<int>()
            .Trace(a => $"{GetHeadingMarker(fragment, a)} {fragment.Header}");

    private static string GetHeadingMarker(HeaderFragment fragment, int level)
        => new('#', level + fragment.Level);

    private static Flow<Flow> Content(ContentFragment fragment) =>
        Pulse.Trace($"{fragment.Content}  ");

    private static Flow<Flow> Code(CodeFragment fragment) =>
        Pulse
            .Trace($"```{fragment.Language}")
            .Trace(fragment.Code.Trim())
            .Trace("```");

    private static Flow<Flow> CodeExample(CodeExampleFragment fragment) =>
         from examples in Pulse.Draw<IReadOnlyCollection<Example>>()
         let example = examples.SingleOrDefault(a => a.Name == fragment.Name)
         from _ in
            Pulse
                .Trace($"```{fragment.Language}")
                .TraceIf(example != null, () => example.Code)
                .When(example == null, () => throw new CodeExampleNotFoundException(fragment.Name))
                .Trace("```")
         select Flow.Continue;

    private static Flow<Flow> Include(InclusionFragment fragment) =>
        from include in Pulse.Draw<IReadOnlyCollection<Inclusion>, Inclusion>(
            a => a.Single(a => a.Type == fragment.Included))
        from _ in
            Pulse
                .ToFlowIf(include.NoHeader, Fragments!, () => include.Explanation.Fragments)
                .ToFlowIf(!include.NoHeader, Explanation!, () => include.Explanation)
        select Flow.Continue;

    private static Flow<Flow> Link(LinkFragment fragment) =>
        from includeLinks in Pulse.Draw<bool>()
        from includes in Pulse.Draw<IReadOnlyCollection<Inclusion>>()
        from _ in
            Pulse
                .Trace("")
                .Trace($"[{fragment.Name}]: {GetLink(fragment, includeLinks)}")
        select Flow.Continue;

    private static Flow<Flow> TableRow(IEnumerable<string> row) =>
        Pulse.Trace($"| {string.Join("| ", row)} |");

    private static Flow<Flow> Row(RowFragment fragment) =>
        Pulse
            .Draw<bool>()
            .ToFlow(TableRow, a => GetCells(fragment, a));

    private static IEnumerable<string> GetCells(RowFragment fragment, bool includeLinks)
    {
        var link = GetLink(fragment.FirstCell, includeLinks);
        var firstCell = $"[{fragment.FirstCell.Content}]({link})";
        return fragment.Cells.Select(a => a.Content).Prepend(firstCell);
    }

    private static string GetLink(ILink fragment, bool includeLinks)
        => includeLinks
            ? fragment.Link
            : fragment.LocalLink;

    private static Flow<Flow> Table(TableFragment fragment) =>
            Pulse
                .ToFlow(TableRow, fragment.Headers)
                .ToFlow(TableRow, fragment.Headers.Select(_ => "-").ToArray())
                .ToFlow(Row, fragment.Body);

    private static Flow<Flow> Fragment(Fragment fragment) =>
        from _ in fragment switch
        {
            HeaderFragment a => Pulse.ToFlow(b => Pulse.ToFlow(Header, b), a),
            ContentFragment a => Pulse.ToFlow(Content, a),
            CodeFragment a => Pulse.ToFlow(Code, a),
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
        Pulse
            .ToFlow(MarkDownHeader, explanation.HeaderText)
            .ToFlow(Fragments, explanation.Fragments);

    private static Flow<Flow> BookPage(Page page) =>
        Pulse.Scoped<int>(a => page.Path.Split(Separator).Length, Pulse.ToFlow(Explanation, page.Explanation));

    // this is the entrypoint for single page doc
    public static Flow<Flow> Book(Book book) =>
        Pulse
            .Prime(() => false).Dissipate()
            .ToFlow(LoadReference, book)
            .ToFlow(BookPage, book.Pages);

    // this is the entry point for writing a doc folder
    public static Flow<Flow> SinglePage(SinglePage pageAndReference) =>
        Pulse
            .Prime(() => true).Dissipate()
            .ToFlow(LoadReference, pageAndReference)
            .Scoped<int>(a => 1, Pulse.ToFlow(Explanation, pageAndReference.Page.Explanation));

    public static Flow<Flow> TableOfContent(Chronicle chronicle) =>
        Pulse.Trace($"{GetIndent(chronicle)}- [{chronicle.Text}]({chronicle.Path})");

    private static string GetIndent(Chronicle chronicle)
    {
        var level = chronicle.Path.Split(Separator).Length - 1;
        return new string(' ', level * 2);
    }
}

