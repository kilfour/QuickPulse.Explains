using QuickPulse.Explains.Tests.CodeParsing.Implementation;
using QuickPulse.Explains.Text;
using QuickPulse.Show;

namespace QuickPulse.Explains.Tests.CodeParsing;

public class CodeSnippetParsing
{
    [Fact]
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
        var result = CodeReader.AsSnippet(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("// just some text { a { b } }", reader.NextLine());
        Assert.Equal("", reader.NextLine()); // <= This should not be here
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void MethodWithBraces_two_lines()
    {
        string[] input =
[
"    [Fact]",
"    private void Foo()",
"    {",
"        // just some text { a { b } }",
"        var x = 0;",
"    }",
"    Nope"
];
        var result = CodeReader.AsSnippet(input).PulseToLog("debug.log");
        var reader = LinesReader.FromText(result);
        Assert.Equal("// just some text { a { b } }", reader.NextLine());
        Assert.Equal("var x = 0;", reader.NextLine());
        Assert.Equal("", reader.NextLine()); // <= This should not be here
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void MethodWithBraces_OneLine()
    {
        string[] input =
[
"    [Fact] private void Foo() { /* just some text */ } Nope"
];
        var result = CodeReader.AsSnippet(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("/* just some text */ ", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact(Skip = "not implemented")]
    public void MethodWithBraces_OneLine_Tricky()
    {
        string[] input =
[
"    [Fact] private void Foo() { /* just some text } */ } Nope"
];
        var result = CodeReader.AsSnippet(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("/* just some text } */ ", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
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
        var result = CodeReader.AsSnippet(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("\"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void MethodWithoutBraces_MultiLine()
    {
        string[] input =
[
"    [Fact]",
"    public static string Foo() ",
"        => ",
"            true",
"                ? 1",
"                : 0;",
"    Nope"
];
        var result = CodeReader.AsSnippet(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("true", reader.NextLine());
        Assert.Equal("    ? 1", reader.NextLine());
        Assert.Equal("    : 0;", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void MethodWithoutBraces_OneLine()
    {
        string[] input =
[
"     [Fact] public static string Foo() =>  \"foo\"; Nope"
];
        var result = CodeReader.AsSnippet(input);
        var reader = LinesReader.FromText(result);
        Assert.Equal("\"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}
