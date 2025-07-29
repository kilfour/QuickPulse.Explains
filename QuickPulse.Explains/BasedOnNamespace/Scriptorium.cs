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

    private static readonly Flow<DocHeaderAttribute> Header =
         from attr in Pulse.Start<DocHeaderAttribute>()
         from _ in Pulse.ToFlow(MarkDownHeader, attr.Header)
         select attr;

    private static readonly Flow<DocContentAttribute> Content =
         from attr in Pulse.Start<DocContentAttribute>()
         from _ in Pulse.Trace(attr.Content)
         select attr;

    private static readonly Flow<DocIncludeAttribute> Include =
         from attr in Pulse.Start<DocIncludeAttribute>()
         from includes in Pulse.Gather<IReadOnlyCollection<Inclusion>>()
         from _ in Pulse.ToFlow(Explanation, includes.Value.Single(a => a.Type == attr.IncludedDoc).Explanation)
         select attr;

    private static readonly Flow<DocMethodAttribute> Method =
        from attr in Pulse.Start<DocMethodAttribute>()
        from _h in Pulse.ToFlowIf(attr is DocHeaderAttribute, Header, () => (DocHeaderAttribute)attr)
        from _c in Pulse.ToFlowIf(attr is DocContentAttribute, Content, () => (DocContentAttribute)attr)
        from _i in Pulse.ToFlowIf(attr is DocIncludeAttribute, Include, () => (DocIncludeAttribute)attr)
        select attr;

    private static Flow<Explanation> Explanation =>
        from input in Pulse.Start<Explanation>()
        from _ in Pulse.ToFlow(MarkDownHeader, input.HeaderText)
        from __ in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Method, input.DocMethods))
        select input;

    private static Flow<Page> Page =>
        from input in Pulse.Start<Page>()
        let level = input.Path.Split(".").Length - 1
        from _ in Pulse.Scoped<int>(a => level,
            from _ in Pulse.ToFlow(MarkDownHeader, input.Explanation.HeaderText)
            from s in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Method, input.Explanation.DocMethods))
            select Unit.Instance)
        select input;

    public static Flow<SeperatePage> SeperatePage =>
        from input in Pulse.Start<SeperatePage>()
        from inc in Pulse.Gather(input.Inclusions)
        from level in Pulse.Gather(0)
        from _ in Pulse.Scoped<int>(a => 1,
            from _ in Pulse.ToFlow(MarkDownHeader, input.Page.Explanation.HeaderText)
            from __ in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Method, input.Page.Explanation.DocMethods))
            select Unit.Instance)
        select input;

    public static Flow<Book> Book =>
        from input in Pulse.Start<Book>()
        from inc in Pulse.Gather(input.Includes)
        from level in Pulse.Gather(1)
        from _ in Pulse.ToFlow(Page, input.Pages)
        select input;

    public static readonly Flow<ToCEntry> ToC =
        from input in Pulse.Start<ToCEntry>()
        from _ in Pulse.Trace($"- [{input.Text}]({input.Path})")
        select input;
}