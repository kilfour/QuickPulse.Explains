namespace QuickPulse.Explains.DontInclude;

[DocFile]
public class Spike
{
    [Fact]
    [CodeFile("Spike.txt", "markdown")]
    [DocExample(typeof(Spike), nameof(Foo))]
    [DocExample(typeof(Spike), nameof(FullFoo))]
    [DocExample(typeof(Bar))]
    [DocExample(typeof(Spike), nameof(AList))]
    [DocExample(typeof(Spike), nameof(AnotherList))]
    public void Files()
    {
        Explain.This<Spike>("Spike.md");
    }

    [CodeSnippet]
    [DocReplace("text", "comment")]
    [DocReplace("some", "a")]
    private void Foo()
    {
        // just some text { a { b } }
    }


    [CodeExample]
    [DocReplace("just some text", "replaced")]
    private void FullFoo()
    {
        // just some text { }
    }


    [CodeExample]
    [DocReplace("Method", "MyMethod")]

    private class Bar
    {
        public int Method() { return 42; }
    }

    [CodeSnippet]
    [DocReplace("return", "")]
    private List<string> AList()
    {
        return
        [
            "char='{', enter=-1, emit=False, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='a', enter=0, emit=True, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='{', enter=0, emit=True, exit=1",
            "char=' ', enter=1, emit=True, exit=1",
            "char='b', enter=1, emit=True, exit=1",
            "char=' ', enter=1, emit=True, exit=1",
            "char='}', enter=1, emit=True, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='c', enter=0, emit=True, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='}', enter=0, emit=True, exit=-1"
        ];
    }

    [CodeExample]
    [DocReplace("return", "")]
    private List<string> AnotherList()
    {
        return
        [
            "char='{', enter=-1, emit=False, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='a', enter=0, emit=True, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='{', enter=0, emit=True, exit=1",
            "char=' ', enter=1, emit=True, exit=1",
            "char='b', enter=1, emit=True, exit=1",
            "char=' ', enter=1, emit=True, exit=1",
            "char='}', enter=1, emit=True, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='c', enter=0, emit=True, exit=0",
            "char=' ', enter=0, emit=True, exit=0",
            "char='}', enter=0, emit=True, exit=-1"
        ];
    }
}

