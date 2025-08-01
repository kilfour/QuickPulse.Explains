using QuickPulse.Explains.BasedOnNamespace;

namespace QuickPulse.Explains.Tests.NamespaceBased;

public class TheCartographerTests
{
    [Fact]
    public void SameNamespace()
    {
        var result = TheCartographer.ChartPath(typeof(ReadMe), typeof(ReadMe));
        Assert.Equal("ReadMe.md", result);
    }

    [Fact]
    public void SubNamespace()
    {
        var result = TheCartographer.ChartPath(typeof(ReadMe), typeof(TheCartographerTests));
        Assert.Equal("NamespaceBased\\TheCartographerTests.md", result);
    }
}