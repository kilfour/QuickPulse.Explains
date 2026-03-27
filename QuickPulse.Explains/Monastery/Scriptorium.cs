using QuickPulse.Explains.Exceptions;
using QuickPulse.Explains.Monastery.Fragments;
using QuickPulse.Explains.Monastery.Fragments.Tables;
using QuickPulse.Explains.Monastery.Writings;

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

    private static string GetErrorMessageFromCodeExampleFragmentName(string name)
    {
        var result = "Code Example not found.\r\n" +
            "Looking for: `" + name + "`.\r\n";
        if (name.Contains('`'))
            result = result +
                " => Are you referencing a generic type?\r\n" +
                "If so you need to supply the open generic type (f.i. `typeof(MyType<>)` instead of `typeof(MyType<string>)`).\r\n";
        result += "-----------------------------------------------------------------------";
        return result;
    }

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
        from _1 in Pulse.ToFlowIf(include.NoHeader, Fragments!, () => include.Explanation.Fragments)
        from _2 in Pulse.ToFlowIf(!include.NoHeader, Explanation!, () => include.Explanation)
        select Flow.Continue;

    private static Flow<Flow> Link(LinkFragment fragment) =>
        from includeLinks in Pulse.Draw<bool>()
        from includes in Pulse.Draw<IReadOnlyCollection<Inclusion>>()
        from newLine in Pulse.Trace("")
        let link = includeLinks
           ? fragment.Location
           : fragment.LocalLocation
        from _2 in Pulse.Trace($"[{fragment.Name}]: {link}")
        select Flow.Continue;


    private static Flow<Flow> TableRow(IEnumerable<string> row) =>
        Pulse.Trace($"| {string.Join("| ", row)} |");

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

