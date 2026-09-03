using QuickPulse.Arteries;
using QuickPulse.Explains.Monastery;

namespace QuickPulse.Explains.Tests.Monastery;

public class IncludeResolutionTests
{
    [Fact]
    public void The_same_type_can_be_included_with_and_without_its_header()
    {
        var output = Render<MixedHeaderRoot>();

        Assert.Equal(1, output.Count(line => line == "## Shared Include"));
        Assert.Equal(2, output.Count(line => line == "shared content  "));
    }

    [Fact]
    public void OnlyThis_resolves_nested_includes()
    {
        var output = Render<RecursiveRoot>();

        Assert.Contains("## Middle Include", output);
        Assert.Contains("### Leaf Include", output);
        Assert.Contains("leaf content  ", output);
    }

    [Fact]
    public void Include_cycles_are_reported_before_rendering()
    {
        var exception = Assert.Throws<InvalidOperationException>(
            TheArchivist.ComposeOnly<CyclicRoot>);

        Assert.Contains("include cycle", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(nameof(CyclicRoot), exception.Message);
        Assert.Contains(nameof(CyclicChild), exception.Message);
    }

    private static IReadOnlyCollection<string> Render<T>()
    {
        var collector = Collect.ValuesOf<string>();
        TheScribe.GetArtery = _ => collector;

        Explain.OnlyThis<T>("ignored.md");

        return collector.Values;
    }

    [DocFile]
    [DocInclude(typeof(SharedInclude))]
    [DocInclude(typeof(SharedInclude), true)]
    private class MixedHeaderRoot;

    [DocContent("shared content")]
    private class SharedInclude;

    [DocFile]
    [DocInclude(typeof(MiddleInclude))]
    private class RecursiveRoot;

    [DocInclude(typeof(LeafInclude))]
    private class MiddleInclude;

    [DocContent("leaf content")]
    private class LeafInclude;

    [DocFile]
    [DocInclude(typeof(CyclicChild))]
    private class CyclicRoot;

    [DocInclude(typeof(CyclicRoot))]
    private class CyclicChild;
}
