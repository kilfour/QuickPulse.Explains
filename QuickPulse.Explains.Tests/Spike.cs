namespace QuickPulse.Explains.DontInclude;

[DocFile]
public class Spike
{
    [Fact]
    [DocCodeFile("Spike.txt", "markdown")]
    [DocCodeExample(typeof(Spike), nameof(Foo))]
    [DocCodeExample(typeof(Spike), nameof(FullFoo))]
    [DocCodeExample(typeof(Bar))]
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


    [DocReplace("just some text", "replaced")]
    [DocExample]
    private void FullFoo()
    {
        // just some text { }
    }


    [DocReplace("Method", "MyMethod")]
    [DocExample]
    private class Bar
    {
        public int Method() { return 42; }
    }
}

