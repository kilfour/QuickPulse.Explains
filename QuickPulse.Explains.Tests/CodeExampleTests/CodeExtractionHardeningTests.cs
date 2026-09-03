using System.Reflection;
using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Monastery.Reading;

namespace QuickPulse.Explains.Tests.CodeExampleTests;

public class CodeExtractionHardeningTests
{
    [Fact]
    public void Dedent_preserves_internal_blank_lines_and_uses_the_smallest_indent()
    {
        var result = TheArchivist.Dedent(
        [
            "",
            "    first",
            "",
            "      second",
            "    third",
            ""
        ], false);

        Assert.Equal(["first", "", "  second", "third"], result);
    }

    [Fact]
    public void Extraction_removes_documentation_attributes_but_preserves_other_attributes()
    {
        var method = typeof(ExampleSources).GetMethod(nameof(ExampleSources.Attributed))!;
        var marker = method.GetCustomAttribute<CodeExampleAttribute>()!;

        var result = CodeExampleExtractor.Extract(marker.File, marker.Line, false);

        Assert.Contains("[Obsolete(\"keep me\")]", result);
        Assert.DoesNotContain("[CodeExample]", result);
    }

    [Fact]
    public void Composition_extracts_only_referenced_examples()
    {
        var book = TheArchivist.ComposeOnly<SelectiveDocument>();

        var example = Assert.Single(book.Examples);
        Assert.Contains(nameof(ExampleSources.Valid), example.Code);
    }

    [Fact]
    public void Extracted_examples_preserve_blank_lines()
    {
        var book = TheArchivist.ComposeOnly<BlankLineDocument>();

        var example = Assert.Single(book.Examples);
        Assert.Contains(Environment.NewLine + Environment.NewLine, example.Code);
    }

    [Fact]
    public void Ambiguous_overloaded_examples_have_a_focused_error()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            TheArchivist.ComposeOnly<AmbiguousDocument>);

        Assert.Contains("More than one code example", exception.Message);
        Assert.Contains(nameof(OverloadedSources.Example), exception.Message);
    }

    [DocFile]
    [DocExample(typeof(ExampleSources), nameof(ExampleSources.Valid))]
    private class SelectiveDocument;

    [DocFile]
    [DocExample(typeof(ExampleSources), nameof(ExampleSources.WithBlankLine))]
    private class BlankLineDocument;

    private class ExampleSources
    {
        [CodeExample]
        public static void Valid()
        {
            _ = 42;
        }

        [CodeExample("missing-source-file.cs", 1)]
        public static void BrokenButUnreferenced()
        {
        }

        [Obsolete("keep me"), CodeExample]
        public static void Attributed()
        {
        }

        [CodeExample]
        public static void WithBlankLine()
        {
            var first = 1;

            var second = 2;
            _ = first + second;
        }
    }

    [DocFile]
    [DocExample(typeof(OverloadedSources), nameof(OverloadedSources.Example))]
    private class AmbiguousDocument;

    private class OverloadedSources
    {
        [CodeExample]
        public static void Example()
        {
        }

        [CodeExample]
        public static void Example(int value)
        {
            _ = value;
        }
    }
}
