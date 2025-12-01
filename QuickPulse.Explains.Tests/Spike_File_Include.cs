namespace QuickPulse.Explains.DontInclude;

[DocFile]
[DocRawFile("test.md")]
public class Spike_File_Include
{
    [Fact]
    public void Content()
    {
        Explain.OnlyThis<Spike_File_Include>("Spike_File_Include.md");
    }
}