using QuickPulse.Bolts;

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

    private static readonly Flow<InclusionFragment> Include =
         from attr in Pulse.Start<InclusionFragment>()
         from includes in Pulse.Gather<IReadOnlyCollection<Inclusion>>()
         from _ in Pulse.ToFlow(Explanation, includes.Value.Single(a => a.Type == attr.Included).Explanation)
         select attr;

    private static readonly Flow<Fragment> Fragment =
        from attr in Pulse.Start<Fragment>()
        from _ in attr switch
        {
            HeaderFragment h => Pulse.ToFlow(a => Pulse.ToFlow(MarkDownHeader, a.Header), h),
            ContentFragment c => Pulse.ToFlow(a => Pulse.Trace(a.Content), c),
            InclusionFragment i => Pulse.ToFlow(Include, i),
            _ => Pulse.NoOp()
        }
        select attr;

    private static Flow<Explanation> Explanation =>
        from input in Pulse.Start<Explanation>()
        from _ in Pulse.ToFlow(MarkDownHeader, input.HeaderText)
        from __ in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Fragment, input.Fragments))
        select input;

    private static Flow<Page> Page =>
        from input in Pulse.Start<Page>()
        let level = input.Path.Split("\\").Length
        from _ in Pulse.Scoped<int>(a => level,
            from _ in Pulse.ToFlow(MarkDownHeader, input.Explanation.HeaderText)
            from s in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Fragment, input.Explanation.Fragments))
            select Unit.Instance)
        select input;

    public static Flow<SeperatePage> SeperatePage =>
        from input in Pulse.Start<SeperatePage>()
        from inc in Pulse.Gather(input.Inclusions)
        from level in Pulse.Gather(0)
        from _ in Pulse.Scoped<int>(a => 1,
            from _ in Pulse.ToFlow(MarkDownHeader, input.Page.Explanation.HeaderText)
            from __ in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Fragment, input.Page.Explanation.Fragments))
            select Unit.Instance)
        select input;

    public static Flow<Book> Book =>
        from input in Pulse.Start<Book>()
        from inc in Pulse.Gather(input.Includes)
        from level in Pulse.Gather(1)
        from _ in Pulse.ToFlow(Page, input.Pages)
        select input;

    public static readonly Flow<Chronicle> Chronicles =
        from input in Pulse.Start<Chronicle>()
        from _ in Pulse.Trace($"- [{input.Text}]({input.Path})")
        select input;
}