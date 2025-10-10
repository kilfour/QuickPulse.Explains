using QuickPulse.Explains.Monastery.Fragments;

namespace QuickPulse.Explains.Monastery;

public static class Scriptorium
{
    public static readonly Flow<Reference> LoadReference =
        from reference in Pulse.Start<Reference>()
        from includes in Pulse.Ensure(() => reference.Inclusions)
        from examples in Pulse.Ensure(() => reference.Examples)
        from level in Pulse.Ensure(() => 1)
        select reference;

    private static readonly Flow<string> MarkDownHeader =
         from text in Pulse.Start<string>()
         from level in Pulse.Extract<int>()
         let headingMarker = new string('#', level)
         let header = $"{headingMarker} {text}"
         from _ in Pulse.Trace(header)
         select text;

    private static readonly Flow<HeaderFragment> Header =
         from fragment in Pulse.Start<HeaderFragment>()
         from level in Pulse.Extract<int>()
         let headingMarker = new string('#', level + fragment.Level)
         from _ in Pulse.Trace($"{headingMarker} {fragment.Header}")
         select fragment;

    private static readonly Flow<ContentFragment> Content =
         from fragment in Pulse.Start<ContentFragment>()
         from _ in Pulse.Trace($"{fragment.Content}  ")
         select fragment;

    private static readonly Flow<CodeFragment> Code =
         from fragment in Pulse.Start<CodeFragment>()
         from s in Pulse.Trace($"```{fragment.Language}")
         from _ in Pulse.Trace(fragment.Code.Trim())
         from e in Pulse.Trace("```")
         select fragment;

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

    private static readonly Flow<CodeExampleFragment> CodeExample =
         from fragment in Pulse.Start<CodeExampleFragment>()
         from examples in Pulse.Extract<IReadOnlyCollection<Example>>()
         let example = examples.SingleOrDefault(a => a.Name == fragment.Name)
         from s in Pulse.Trace($"```{fragment.Language}")
         from _ in Pulse.TraceIf(example != null, () => example.Code)
         from __ in Pulse.When(example == null,
            () => Pulse.TraceTo<Diagnostics>(GetErrorMessageFromCodeExampleFragmentName(fragment.Name)))
         from e in Pulse.Trace("```")
         select fragment;

    private static Flow<InclusionFragment> Include =>
         from fragment in Pulse.Start<InclusionFragment>()
         from includes in Pulse.Extract<IReadOnlyCollection<Inclusion>>()
         from _ in Pulse.ToFlow(Explanation, includes.Single(a => a.Type == fragment.Included).Explanation)
         select fragment;

    private static readonly Flow<Fragment> Fragment =
        from fragment in Pulse.Start<Fragment>()
        from _ in fragment switch
        {
            HeaderFragment a => Pulse.ToFlow(b => Pulse.ToFlow(Header, b), a),
            ContentFragment a => Pulse.ToFlow(Content, a),
            CodeFragment a => Pulse.ToFlow(Code, a),
            CodeExampleFragment a => Pulse.ToFlow(CodeExample, a),
            InclusionFragment a => Pulse.ToFlow(Include, a),
            _ => Pulse.NoOp()
        }
        select fragment;

    private static readonly Flow<Explanation> Explanation =
        from explanation in Pulse.Start<Explanation>()
        from _ in Pulse.ToFlow(MarkDownHeader, explanation.HeaderText)
        from __ in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Fragment, explanation.Fragments))
        select explanation;

    private static readonly Flow<Page> Page =
        from page in Pulse.Start<Page>()
        let level = page.Path.Split("/").Length // Create a Path Seperator Constant somewhere
        from _ in Pulse.Scoped<int>(a => level, Pulse.ToFlow(Explanation, page.Explanation))
        select page;

    public static readonly Flow<SeperatePage> SeperatePage =
        from typeset in Pulse.Start<SeperatePage>()
        from initialize in Pulse.ToFlow(LoadReference, typeset)
        from _ in Pulse.Scoped<int>(a => 1, Pulse.ToFlow(Explanation, typeset.Page.Explanation))
        select typeset;

    public static readonly Flow<Book> Book =
        from book in Pulse.Start<Book>()
        from initialize in Pulse.ToFlow(LoadReference, book)
        from _ in Pulse.ToFlow(Page, book.Pages)
        select book;

    public static readonly Flow<Chronicle> Chronicles =
        from chronicle in Pulse.Start<Chronicle>()
        let level = chronicle.Path.Split("/").Length - 1
        let indent = new string(' ', level * 2)
        from _ in Pulse.Trace($"{indent}- [{chronicle.Text}]({chronicle.Path})")
        select chronicle;
}