using QuickPulse.Explains.Formatters;

namespace QuickPulse.Explains.DontInclude;

[DocFile]
[DocExample(typeof(Bar))]
public class Spike
{

    //[Fact]
    [Fact(Skip = "explicit")]
    // [DocExample(typeof(Spike), "NopeNotHere")]
    // [DocExample(typeof(Generic<string>))]
    public void Content()
    {
        Explain.OnlyThis<Spike>("Spike.md");
    }

    [CodeExample]
    public class Generic<T> { }

    [DocCodeFile("Spike.txt", "markdown")]
    [DocExample(typeof(Spike), nameof(Foo))]
    [DocExample(typeof(Spike), nameof(FullFoo))]
    [DocExample(typeof(Bar))]
    [DocExample(typeof(Spike), nameof(AList), "sql")]
    [DocExample(typeof(Spike), nameof(AnotherList))]
    private void Files()
    {
        // ----
    }

    [CodeSnippet]
    [CodeReplace("text", "comment")]
    [CodeReplace("some", "a")]
    private void Foo()
    {
        // just some text { a { b } }
    }


    [CodeExample]
    [CodeReplace("just some text", "replaced")]
    private void FullFoo()
    {
        // just some text { }
    }


    [CodeExample]
    [CodeReplace("Method", "MyMethod")]

    private class Bar
    {
        public int Method() { return 42; }
    }

    [CodeSnippet]
    [CodeFormat(typeof(StringArrayToString))]
    private List<string> AList()
    {
        return
        [
            "one\"withquote",
            "tw,o",
            "three"
        ];
    }

    [CodeExample]
    [CodeReplace("return", "")]
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

[DocFile]
[DocExample(typeof(Bar))]
public class Foo
{
    [CodeExample]
    private class Bar
    {
        public int Method() { return 42; }
    }
}

