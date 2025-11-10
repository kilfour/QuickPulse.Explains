namespace QuickPulse.Explains.Tests;

[DocFile]
public class Spike_Snippet
{
    [CodeSnippet]
    private string SampleField =
        "This is a sample field.";


    [Fact]
    [DocExample(typeof(Spike_Snippet), nameof(SampleField))]
    public void TestSampleField()
    {
        Explain.OnlyThis<Spike_Snippet>("./QuickPulse.Explains.Tests/Spike_Snippet.md");
    }
}