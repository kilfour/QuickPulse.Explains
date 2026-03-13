namespace QuickPulse.Explains.DontInclude;

[DocFile]
[DocRawFile("test.md")]
public class Spike_File_Include_RawFile
{
    // [Fact]
    [Fact(Skip = "explicit")]
    public void Content()
    {
        Explain.This<Spike_File_Include_RawFile>("Spike_File_Include.md");
    }
}