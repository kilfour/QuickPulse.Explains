using QuickPulse.Bolts;
using QuickPulse.Explains.BasedOnNamespace.Fragments;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class Scriptorium
{
    private static readonly Flow<string> MarkDownHeader =
         from text in Pulse.Start<string>()
         from level in Pulse.Gather<int>()
         let headingMarker = new string('#', level.Value)
         let header = $"{headingMarker} {text}"
         from _ in Pulse.Trace(header)
         select text;

    private static readonly Flow<HeaderFragment> HeaderFragment =
         from header in Pulse.Start<HeaderFragment>()
         from level in Pulse.Gather<int>()
         let headingMarker = new string('#', level.Value + header.Level)
         from _ in Pulse.Trace($"{headingMarker} {header.Header}")
         select header;

    private static readonly Flow<CodeFragment> Code =
         from fragment in Pulse.Start<CodeFragment>()
         from s in Pulse.Trace($"```{fragment.Language}")
         from _ in Pulse.Trace(fragment.Code.Trim())
         from e in Pulse.Trace("```")
         select fragment;



    private static readonly Flow<CodeExampleFragment> CodeExample =
         from fragment in Pulse.Start<CodeExampleFragment>()
         from examples in Pulse.Gather<IReadOnlyCollection<Example>>()
         from s in Pulse.Trace($"```csharp")
         from _ in Pulse.Trace(examples.Value.Single(a => a.Name == fragment.Name).Code)
         from e in Pulse.Trace("```")
         select fragment;

    private static readonly Flow<InclusionFragment> Include =
         from fragment in Pulse.Start<InclusionFragment>()
         from includes in Pulse.Gather<IReadOnlyCollection<Inclusion>>()
         from _ in Pulse.ToFlow(Explanation, includes.Value.Single(a => a.Type == fragment.Included).Explanation)
         select fragment;

    private static readonly Flow<Fragment> Fragment =
        from fragment in Pulse.Start<Fragment>()
        from _ in fragment switch
        {
            HeaderFragment a => Pulse.ToFlow(b => Pulse.ToFlow(HeaderFragment, b), a),
            ContentFragment a => Pulse.ToFlow(b => Pulse.Trace(b.Content), a),
            CodeFragment a => Pulse.ToFlow(Code, a),
            CodeExampleFragment a => Pulse.ToFlow(CodeExample, a),
            InclusionFragment a => Pulse.ToFlow(Include, a),
            _ => Pulse.NoOp()
        }
        select fragment;

    private static Flow<Explanation> Explanation =>
        from input in Pulse.Start<Explanation>()
        from _ in Pulse.ToFlow(MarkDownHeader, input.HeaderText)
        from __ in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Fragment, input.Fragments))
        select input;

    private static Flow<Page> Page =>
        from input in Pulse.Start<Page>()
        let level = input.Path.Split("/").Length // Create a Path Seperator Constant somewhere
        from _ in Pulse.Scoped<int>(a => level,
            from _ in Pulse.ToFlow(MarkDownHeader, input.Explanation.HeaderText)
            from s in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Fragment, input.Explanation.Fragments))
            select Unit.Instance)
        select input;

    public static Flow<SeperatePage> SeperatePage =>
        from input in Pulse.Start<SeperatePage>()
        from includes in Pulse.Gather(input.Inclusions)
        from examples in Pulse.Gather(input.Examples)
        from level in Pulse.Gather(0)
        from _ in Pulse.Scoped<int>(a => 1,
            from _ in Pulse.ToFlow(MarkDownHeader, input.Page.Explanation.HeaderText)
            from __ in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Fragment, input.Page.Explanation.Fragments))
            select Unit.Instance)
        select input;

    public static Flow<Book> Book =>
        from input in Pulse.Start<Book>()
        from includes in Pulse.Gather(input.Includes)
        from examples in Pulse.Gather(input.Examples)
        from level in Pulse.Gather(1)
        from _ in Pulse.ToFlow(Page, input.Pages)
        select input;

    public static readonly Flow<Chronicle> Chronicles =
        from input in Pulse.Start<Chronicle>()
        let level = input.Path.Split("/").Count() - 1
        let indent = new string(' ', level * 2)
        from _ in Pulse.Trace($"{indent}- [{input.Text}]({input.Path})")
        select input;
}