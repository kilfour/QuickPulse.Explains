namespace QuickPulse.Explains.DontInclude;

[DocFile]
[DocLink("LinkName", typeof(Spike_References))]
[DocLink("Anchored", typeof(Spike_References), "Section")]
public class Spike_References
{
    //[Fact]
    [Fact(Skip = "explicit")]
    [DocHeader("Section")]
    public void Try()
    {
        Explain.OnlyThis<Spike_References>("SpikeReferences.md");
    }
}