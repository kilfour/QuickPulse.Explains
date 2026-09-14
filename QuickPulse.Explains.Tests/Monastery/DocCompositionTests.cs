using QuickPulse.Arteries;
using QuickPulse.Explains.Abstractions;
using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Monastery.Fragments;

namespace QuickPulse.Explains.Tests.Monastery;

public class DocCompositionTests
{
    [Fact]
    public void Composites_preserve_class_and_method_content_order()
    {
        var book = TheArchivist.ComposeOnly<OrderedDocument>();
        var fragments = Assert.Single(book.Pages).Explanation.Fragments;
        Assert.Equal(["before", "class one", "class two", "after", "method one", "method two"],
            fragments.Cast<ContentFragment>().Select(fragment => fragment.Content));
    }

    [Fact]
    public void Expanded_includes_and_examples_render_and_expand_once_per_occurrence()
    {
        var collector = Collect.ValuesOf<string>();
        var previous = TheScribe.GetArtery;
        try
        {
            IncludeOnceAttribute.ExpansionCount.Value = 0;
            TheScribe.GetArtery = _ => collector;
            Explain.OnlyThis<CompositeDocument>("ignored.md");
            Assert.Equal(2, IncludeOnceAttribute.ExpansionCount.Value);
            var output = string.Join("\n", collector.Values);
            Assert.Contains("## Examples", output);
            Assert.Contains("leaf content", output);
            Assert.Contains("return 42;", output);
        }
        finally
        {
            TheScribe.GetArtery = previous;
        }
    }

    [Fact]
    public void Expanded_include_cycles_are_reported()
    {
        var exception = Assert.Throws<InvalidOperationException>(TheArchivist.ComposeOnly<CyclicDocument>);
        Assert.Contains("include cycle", exception.Message);
    }

    [Fact]
    public void Include_discovery_sees_composite_attributes()
    {
        var include = Assert.Single(TheReflectionist.GetIncludedTypes([typeof(CompositeDocument)]));
        Assert.Equal(typeof(Middle), include.Type);
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    private sealed class ParagraphsAttribute(string prefix) : Attribute, IDocAttribute
    {
        public IEnumerable<DocFragmentAttribute> Expand() =>
            [new DocContentAttribute(prefix + " one"), new DocContentAttribute(prefix + " two")];
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    private sealed class IncludeOnceAttribute(Type included) : Attribute, IDocAttribute
    {
        public static readonly AsyncLocal<int> ExpansionCount = new();
        private bool expanded;
        public IEnumerable<DocFragmentAttribute> Expand()
        {
            if (expanded) throw new InvalidOperationException("Expanded twice.");
            expanded = true;
            ExpansionCount.Value++;
            yield return new DocIncludeAttribute(included);
        }
    }

    private sealed class ExamplesAttribute : Attribute, IDocAttribute
    {
        public IEnumerable<DocFragmentAttribute> Expand() =>
            [new DocHeaderAttribute("Examples"), new DocExampleAttribute(typeof(Source), nameof(Source.Answer))];
    }

    [DocFile]
    [DocContent("before")]
    [Paragraphs("class")]
    [DocContent("after")]
    private class OrderedDocument
    {
        [Paragraphs("method")]
        public void Method() { }
    }

    [DocFile]
    [IncludeOnce(typeof(Middle))]
    private class CompositeDocument;

    private class Middle
    {
        [IncludeOnce(typeof(Leaf))]
        [Examples]
        public void Method() { }
    }

    [DocContent("leaf content")]
    private class Leaf;

    private class Source
    {
        [CodeExample]
        public static int Answer() { return 42; }
    }

    [DocFile]
    [IncludeOnce(typeof(CyclicDocument))]
    private class CyclicDocument;
}
