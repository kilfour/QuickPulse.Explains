using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests.CodeParsing;

public class CodeExampleParsing
{
    [Fact(Skip = "wip")]
    public void MethodWithBraces()
    {
        string[] input =
[
"    [Fact]",
"    private void Foo()",
"    {",
"        // just some text { a { b } }",
"    }",
"    Nope"
];
        var result = CodeReader.Process(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("private void Foo()", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    // just some text { a { b } }", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact(Skip = "wip")]
    public void MethodWithBraces_OneLine()
    {
        string[] input =
[
"    [Fact] private void Foo() { /* just some text } */ } Nope"
];
        var result = CodeReader.Process(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("private void Foo() { /* just some text } */ }", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact(Skip = "wip")]
    public void MethodWithoutBraces()
    {
        string[] input =
[
"    [Fact]",
"    public static string Foo() ",
"        => ",
"            \"foo\";",
"    Nope"
];
        var result = CodeReader.Process(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("public static string Foo() ", reader.NextLine());
        Assert.Equal("    => ", reader.NextLine());
        Assert.Equal("        \"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact(Skip = "wip")]
    public void MethodWithoutBraces_OneLine()
    {
        string[] input =
[
"     [Fact] public static string Foo() =>  \"foo\"; Nope"
];
        var result = CodeReader.Process(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("public static string Foo() =>  \"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}
