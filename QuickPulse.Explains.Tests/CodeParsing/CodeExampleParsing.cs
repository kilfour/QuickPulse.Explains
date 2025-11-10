using QuickPulse.Explains.Monastery.Reading;
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
"    [A] [B] private void Foo() { /* just some text */ } Nope",
"    [A] [B] private void Bar() { /* just some text */ } Nope"
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

    [Fact]
    public void Field()
    {
        string[] input =
[
"    [A]",
"    [B]",
"    public static string Foo() ",
"        = ",
"            \"foo\";",
"    Nope"
];
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("public static string Foo() ", reader.NextLine());
        Assert.Equal("    = ", reader.NextLine());
        Assert.Equal("        \"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Field_OneLine()
    {
        string[] input =
[
"     [Fact] public static string Foo() =  \"foo\"; Nope"
];
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("public static string Foo() =  \"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Field_JustChecking()
    {
        string[] input =
[
"    public static readonly FuzzrOf<string> SSNFuzzr = ",
"       from a in Fuzzr.Int(100, 999)",
"       from b in Fuzzr.Int(10, 99)",
"       from c in Fuzzr.Int(1000, 9999)",
"       select $\"{a}-{b}-{c}\";"
];
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("public static readonly FuzzrOf<string> SSNFuzzr = ", reader.NextLine());
        Assert.Equal("   from a in Fuzzr.Int(100, 999)", reader.NextLine());
        Assert.Equal("   from b in Fuzzr.Int(10, 99)", reader.NextLine());
        Assert.Equal("   from c in Fuzzr.Int(1000, 9999)", reader.NextLine());
        Assert.Equal("   select $\"{a}-{b}-{c}\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Field_JustChecking_again()
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
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("public static readonly FuzzrOf<string> SSNFuzzr =", reader.NextLine());
        Assert.Equal("   from a in Fuzzr.Int(100, 999)", reader.NextLine());
        Assert.Equal("   from b in Fuzzr.Int(10, 99)", reader.NextLine());
        Assert.Equal("   from c in Fuzzr.Int(1000, 9999)", reader.NextLine());
        Assert.Equal("   select $\"{a}-{b}-{c}\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Field_JustChecking_simpler()
    {
        string[] input =
[
"    public static readonly FuzzrOf<string> SSNFuzzr =",
"       from a in Fuzzr.Int(100, 999)",
"       from b in Fuzzr.Int(10, 99)",
"       from c in Fuzzr.Int(1000, 9999)",
"       select $\"foo\";"
];
        var result = CodeReader.AsExample(input);
        var reader = LinesReader.FromStringList([.. result]);
        Assert.Equal("public static readonly FuzzrOf<string> SSNFuzzr =", reader.NextLine());
        Assert.Equal("   from a in Fuzzr.Int(100, 999)", reader.NextLine());
        Assert.Equal("   from b in Fuzzr.Int(10, 99)", reader.NextLine());
        Assert.Equal("   from c in Fuzzr.Int(1000, 9999)", reader.NextLine());
        Assert.Equal("   select $\"foo\";", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}