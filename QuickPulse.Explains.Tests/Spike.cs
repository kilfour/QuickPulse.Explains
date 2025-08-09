namespace QuickPulse.Explains.DontInclude;

[DocFile]
public class Spike
{
    [Fact]
    [DocCodeFile("Spike.txt", "markdown")]
    public void Files()
    {
        Explain.This<Spike>("Spike.md");
    }
}