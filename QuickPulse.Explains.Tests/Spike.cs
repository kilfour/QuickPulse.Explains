namespace QuickPulse.Explains.DontInclude;

[DocFile]
public class Spike
{
    [Fact]
    [DocCodeFile("Spike.txt", "markdown")]
    [DocCodeExample(typeof(Spike), nameof(Foo))]
    [DocCodeExample(typeof(Spike), nameof(FullFoo))]
    [DocCodeExample(typeof(Bar))]
    [DocCodeExample(typeof(Spike), nameof(AList))]
    [DocCodeExample(typeof(Spike), nameof(AnotherList))]
    public void Files()
    {
        Explain.This<Spike>("Spike.md");
    }

    [DocSnippet]
    [DocReplace("text", "comment")]
    [DocReplace("some", "a")]
    private void Foo()
    {
        // just some text { a { b } }
    }


    [DocExample]
    [DocReplace("just some text", "replaced")]
    private void FullFoo()
    {
        // just some text { }
    }


    [DocExample]
    [DocReplace("Method", "MyMethod")]

    private class Bar
    {
        public int Method() { return 42; }
    }

    [DocSnippet]
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

    [DocExample]
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

