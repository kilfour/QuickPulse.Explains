using System.Runtime.CompilerServices;
using QuickPulse.Explains.Abstractions;
using QuickPulse.Explains.Formatters;
using QuickPulse.Explains.Monastery;

namespace QuickPulse.Explains.Tests.CodeExampleTests;

public class CodeCompositionTests
{
    [Fact]
    public void Composite_examples_and_snippets_use_the_call_site_and_ordered_transformations()
    {
        ExampleBundleAttribute.Expansions.Value = 0;
        var book = TheArchivist.ComposeOnly<Document>();

        Assert.Equal(1, ExampleBundleAttribute.Expansions.Value);
        Assert.Equal(2, book.Examples.Count);
        var example = book.Examples.Single(item => item.Name.EndsWith(".Example"));
        Assert.Contains("public static string Example()", example.Code);
        Assert.Contains("[Obsolete(\"keep\")]", example.Code);
        Assert.Contains("return \"finished\";", example.Code);
        Assert.DoesNotContain("ExampleBundle", example.Code);
        Assert.DoesNotContain("Cleanup", example.Code);
        Assert.DoesNotContain("CodeReplace", example.Code);
        var snippet = book.Examples.Single(item => item.Name.EndsWith(".Snippet"));
        Assert.Equal("return 42;", snippet.Code);
    }

    [Fact]
    public void Composite_class_markers_extract_the_class()
    {
        var example = Assert.Single(TheArchivist.ComposeOnly<ClassDocument>().Examples);
        Assert.Contains("class ClassSource", example.Code);
        Assert.Contains("Value = 42", example.Code);
        Assert.DoesNotContain("ExampleBundle", example.Code);
    }

    [Fact]
    public void Unreferenced_composites_are_not_expanded()
    {
        Assert.Single(TheArchivist.ComposeOnly<SelectiveDocument>().Examples);
    }

    private sealed class BrokenBundleAttribute : Attribute, ICodeAttribute
    {
        public IEnumerable<CodeAttribute> Expand() => throw new InvalidOperationException("Unused composite expanded.");
    }

    [DocFile]
    [DocExample(typeof(SelectiveSource), nameof(SelectiveSource.Used))]
    private class SelectiveDocument;

    private class SelectiveSource
    {
        [CodeExample]
        public static int Used() => 42;

        [BrokenBundle]
        public static void Unused() { }
    }

    [Fact]
    public void Composite_and_direct_markers_are_rejected_when_ambiguous()
    {
        var exception = Assert.Throws<InvalidOperationException>(TheArchivist.ComposeOnly<AmbiguousDocument>);
        Assert.Contains("More than one code example", exception.Message);
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    private sealed class ExampleBundleAttribute(
        [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) : Attribute, ICodeAttribute
    {
        public static readonly AsyncLocal<int> Expansions = new();
        public IEnumerable<CodeAttribute> Expand()
        {
            Expansions.Value++;
            yield return new CodeExampleAttribute(file, line);
            yield return new CodeReplaceAttribute("first", "second");
        }
    }

    private sealed class CleanupAttribute : Attribute, ICodeAttribute
    {
        public IEnumerable<CodeAttribute> Expand() =>
            [new CodeReplaceAttribute("second", "third"), new CodeRemoveAttribute("REMOVE"),
                new CodeFormatAttribute(typeof(FinishFormatter))];
    }

    private sealed class SnippetBundleAttribute(
        [CallerFilePath] string file = "", [CallerLineNumber] int line = 0) : Attribute, ICodeAttribute
    {
        public IEnumerable<CodeAttribute> Expand() => [new CodeSnippetAttribute(file, line)];
    }

    public sealed class FinishFormatter : ICodeFormatter
    {
        public IEnumerable<string> Format(IEnumerable<string> code) =>
            code.Select(line => line.Replace("fourth", "finished"));
    }

    [DocFile]
    [DocExample(typeof(Source), nameof(Source.Example))]
    [DocExample(typeof(Source), nameof(Source.Snippet))]
    private class Document;

    private class Source
    {
        [Obsolete("keep")]
        [ExampleBundle]
        [Cleanup]
        [CodeReplace("third", "fourth")]
        public static string Example() { return "firstREMOVE"; }

        [SnippetBundle]
        public static int Snippet()
        {
            return 42;
        }
    }

    [DocFile]
    [DocExample(typeof(ClassSource))]
    private class ClassDocument;

    [ExampleBundle]
    private class ClassSource
    {
        public const int Value = 42;
    }

    [DocFile]
    [DocExample(typeof(AmbiguousSource), nameof(AmbiguousSource.Example))]
    private class AmbiguousDocument;

    private class AmbiguousSource
    {
        [CodeExample]
        [ExampleBundle]
        public static void Example() { }
    }
}
