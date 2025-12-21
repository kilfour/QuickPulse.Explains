using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Tests.DocTests;
using QuickPulse.Explains.Tests.Spike_Refs_Again;
using QuickPulse.Explains.Tests.Spike_Refs_Again.Included;
using QuickPulse.Explains.Tests.Spike_Refs_Again.Includor;

namespace QuickPulse.Explains.Tests.Monastery;

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
        Assert.Equal("QuickPulse/Explains/Tests/Monastery/TheCartographerTests.md", result);
    }

    [Fact]
    public void SiblingNamespaces()
    {
        var result = TheCartographer.ChartPath(typeof(ReferencingTypeInOtherNamespace), typeof(FileToLinkTo));
        Assert.Equal("QuickPulse/Explains/Tests/Spike_Refs_Again/Included/FileToLinkTo.md", result);
    }

    [Fact]
    public void ParentNamespace()
    {
        var result = TheCartographer.ChartPath(typeof(FileToLinkTo), typeof(CreateFiles));
        Assert.Equal("QuickPulse/Explains/Tests/Spike_Refs_Again/CreateFiles.md", result);
    }


    [Fact]
    public void LinkSameNamespace()
    {
        var result = TheCartographer.ChartLinkPath(typeof(ReadMe), typeof(ReadMe));
        Assert.Equal("ReadMe.md", result);
    }

    [Fact]
    public void LinkSubNamespace()
    {
        var result = TheCartographer.ChartLinkPath(typeof(ReadMe), typeof(TheCartographerTests));
        Assert.Equal("../Monastery/TheCartographerTests.md", result);
    }

    [Fact]
    public void LinkSiblingNamespaces()
    {
        var result = TheCartographer.ChartLinkPath(typeof(ReferencingTypeInOtherNamespace), typeof(FileToLinkTo));
        Assert.Equal("../Included/FileToLinkTo.md", result);
    }

    [Fact]
    public void LinkParentNamespace()
    {
        var result = TheCartographer.ChartLinkPath(typeof(FileToLinkTo), typeof(CreateFiles));
        Assert.Equal("../CreateFiles.md", result);
    }
}