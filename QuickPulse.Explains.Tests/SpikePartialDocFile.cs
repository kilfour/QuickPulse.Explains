namespace QuickPulse.Explains.DontInclude;

[DocFile]
public class SpikePartialDocFile
{
    [Fact]
    //[Fact(Skip = "explicit")]
    [DocCodeFile("Spike.txt", "markdown", 1, 2)]
    public void Content()
    {
        Explain.OnlyThis<SpikePartialDocFile>("SpikePartialDocFile.md");
    }
}

