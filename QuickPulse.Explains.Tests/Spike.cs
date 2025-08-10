namespace QuickPulse.Explains.DontInclude;

[DocFile]
public class Spike
{
    [Fact]
    [DocCodeFile("Spike.txt", "markdown")]
    [DocCodeExample(typeof(Spike), nameof(Foo))]
    public void Files()
    {
        Explain.This<Spike>("Spike.md");
    }

    [DocExample]
    [DocReplace("text", "comment")]
    [DocReplace("some", "a")]
    private void Foo()
    {
        // just some text
    }
}