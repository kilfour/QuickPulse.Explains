using QuickPulse.Arteries;
using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Tests.CodeExampleTests.Methods.Models;
using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests.CodeExampleTests.Methods.Tests;

public class CodeExampleMethodsTests
{
    [Fact]
    public void Check()
    {
        var collector = Collect.ValuesOf<string>();
        TheScribe.GetArtery = a => collector;
        Explain.This<MethodsModel>("Whatever");
        var reader = LinesReader.FromStringList([.. collector.Values]);
        // reader.AsAssertsToLogFile();

        Assert.Equal("# Methods Model", reader.NextLine());

        Assert.Equal("```csharp", reader.NextLine());
        Assert.Equal("public int MethodOne() { return 42; }", reader.NextLine());
        Assert.Equal("```", reader.NextLine());

        Assert.Equal("```csharp", reader.NextLine());
        Assert.Equal(
@"public int MethodTwo()
{
    return 42;
}", reader.NextLine());
        Assert.Equal("```", reader.NextLine());

        Assert.Equal("```csharp", reader.NextLine());
        Assert.Equal("public int MethodThree() => 42;", reader.NextLine());
        Assert.Equal("```", reader.NextLine());

        Assert.Equal("```csharp", reader.NextLine());
        Assert.Equal(
@"public int MethodFour()
    => 42;", reader.NextLine());
        Assert.Equal("```", reader.NextLine());

        Assert.True(reader.EndOfContent());
    }

    // [DocFile]
    // [DocExample(typeof(MethodsModel), nameof(MethodsModel.MethodFour))]
    // public class MethodOneSut { }

    [Fact]
    public void Debug()
    {
        var result = TheArchivist.Dedent(["    public int MethodFour()", "        => 42;"], false);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("public int MethodFour()", reader.NextLine());
        Assert.Equal("    => 42;", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}