using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Tests.DocTests;

namespace QuickPulse.Explains.Tests.NamespaceBased;

public class TheCartographerTests
{
    [Fact(Skip = "need a better way")]
    public void SameNamespace()
    {
        var result = TheCartographer.ChartPath(typeof(ReadMe), typeof(ReadMe));
        Assert.Equal("ReadMe.md", result);
    }

    [Fact(Skip = "need a better way")]
    public void SubNamespace()
    {
        var result = TheCartographer.ChartPath(typeof(ReadMe), typeof(TheCartographerTests));
        Assert.Equal("NamespaceBased\\TheCartographerTests.md", result);
    }
}