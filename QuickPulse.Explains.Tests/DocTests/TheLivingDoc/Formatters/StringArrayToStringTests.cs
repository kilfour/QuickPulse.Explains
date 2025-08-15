using QuickPulse.Explains.Formatters;

namespace QuickPulse.Explains.Tests.DocTests.TheLivingDoc.Formatters;

public class StringArrayToStringTests
{
    [Fact]
    public void Empty()
    {
        var input = "return [ ];";
        var result = new StringArrayToString().Format([input]);
        Assert.Equal([], result);
    }

    [Fact]
    public void Simple()
    {
        var input = "return [\"hello\"];";
        var result = new StringArrayToString().Format([input]);
        Assert.Equal(["hello"], result);
    }

    [Fact]
    public void Multiline()
    {
        var input = "return \r\n[\r\n    \"hello\",\r\n    \"world\"\r\n    ];";
        var result = new StringArrayToString().Format([input]);
        Assert.Equal(["hello", "world"], result);
    }

    [Fact]
    public void WhatAboutInts()
    {
        var input = "return [ 42 ];";
        var result = new StringArrayToString().Format([input]);
        Assert.Equal([], result);
    }
}