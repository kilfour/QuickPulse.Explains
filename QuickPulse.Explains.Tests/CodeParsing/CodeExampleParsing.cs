using QuickPulse.Explains.Monastery.CodeLocator;
using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests.CodeParsing;

public class CodeExampleParsing
{
    [Fact]
    public void MethodWithBraces()
    {
        string[] input =
[
"    [X(Y = new[] { 1, 2 })]",
"    [Generic(typeof(Dictionary<string, List<int>>))]",
"    private void Foo()",
"    {",
"        // just some text { a { b } }",
"    }",
"    Nope"
];
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("private void Foo()", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    // just some text { a { b } }", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void MethodWithBraces_OneLine()
    {
        string[] input =
[
"    [A] [B] private void Foo() { /* just some text */ } Nope"
];
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("private void Foo() { /* just some text */ }", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact(Skip = "not implemented")]
    public void MethodWithBraces_OneLine_Tricky()
    {
        string[] input =
[
"    [Fact] private void Foo() { /* just some text } */ } Nope"
];
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("private void Foo() { /* just some text } */ }", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void MethodWithoutBraces()
    {
        string[] input =
[
"    [A]",
"    [B]",
"    public static string Foo() ",
"        => ",
"            \"foo\";",
"    Nope"
];
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("public static string Foo() ", reader.NextLine());
        Assert.Equal("    => ", reader.NextLine());
        Assert.Equal("        \"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void MethodWithoutBraces_OneLine()
    {
        string[] input =
[
"     [Fact] public static string Foo() =>  \"foo\"; Nope"
];
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("public static string Foo() =>  \"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}
