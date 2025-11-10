using QuickPulse.Explains.Monastery.Reading;
using QuickPulse.Explains.Text;


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
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("// just some text { a { b } }", reader.NextLine());
        //Assert.Equal("", reader.NextLine()); // <= This should not be here
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
"    [Fact]",
"    private void Bar()",
"    {",
"        // SHOULD NOT SHOW UP",
"        var x = 0;",
"    }",
"    Nope"
];
        var result = CodeReader.AsSnippet(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("// just some text { a { b } }", reader.NextLine());
        Assert.Equal("var x = 0;", reader.NextLine());
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
        var reader = LinesReader.FromStringList([.. result]);
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
        var reader = LinesReader.FromStringList([.. result]);
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
        var reader = LinesReader.FromStringList([.. result]);
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
        var reader = LinesReader.FromStringList([.. result]);
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
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("\"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact(Skip = "not implemented")]
    public void Field_JustChecking()
    {
        string[] input =
[
"    public static readonly FuzzrOf<string> SSNFuzzr =",
"       from a in Fuzzr.Int(100, 999)",
"       from b in Fuzzr.Int(10, 99)",
"       from c in Fuzzr.Int(1000, 9999)",
"       select $\"{a}-{b}-{c}\";",
"       Nope"
];
        var result = CodeReader.AsSnippet(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("from a in Fuzzr.Int(100, 999)", reader.NextLine());
        Assert.Equal("from b in Fuzzr.Int(10, 99)", reader.NextLine());
        Assert.Equal("from c in Fuzzr.Int(1000, 9999)", reader.NextLine());
        Assert.Equal("select $\"{a}-{b}-{c}\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}
