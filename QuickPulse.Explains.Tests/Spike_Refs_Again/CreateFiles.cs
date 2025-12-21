namespace QuickPulse.Explains.Tests.Spike_Refs_Again;

public class CreateFiles
{
    [Fact(Skip = "Explicit")]
    public void Now()
    {
        Explain.These<CreateFiles>("ReferencingTypeInOtherNamespace/");
    }
}