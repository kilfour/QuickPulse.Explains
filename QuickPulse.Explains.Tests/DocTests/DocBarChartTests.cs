using QuickPulse.Arteries;
using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Monastery.Writings;
using QuickPulse.Explains.Text;
using System.Reflection;

namespace QuickPulse.Explains.Tests.DocTests;

public class DocBarChartTests
{
    [Fact]
    [DocBarChart(
        typeof(CurveData),
        nameof(CurveData.WeightCurve),
        "Weight Curve",
        "Maximum Output",
        "Weight",
        0, 6)]
    public void Renders_values_as_a_mermaid_bar_chart()
    {
        var attribute = GetType()
            .GetMethod(nameof(Renders_values_as_a_mermaid_bar_chart))!
            .GetCustomAttribute<DocBarChartAttribute>()!;
        var reader = Render(attribute);
        Assert.Equal("# Chart", reader.NextLine());
        Assert.Equal("```mermaid", reader.NextLine());
        Assert.Equal("xychart-beta", reader.NextLine());
        Assert.Equal("    title \"Weight Curve\"", reader.NextLine());
        Assert.Equal("    x-axis \"Maximum Output\" [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]", reader.NextLine());
        Assert.Equal("    y-axis \"Weight\" 0 --> 6", reader.NextLine());
        Assert.Equal("    bar [3, 3, 3, 3, 3, 4, 4, 5, 6, 6]", reader.NextLine());
        Assert.Equal("```", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Omits_the_y_axis_range_when_it_is_not_supplied()
    {
        var attribute = new DocBarChartAttribute(
            typeof(CurveData),
            nameof(CurveData.WeightCurve),
            "Weight Curve",
            "Maximum Output",
            "Weight");
        var reader = Render(attribute);

        reader.Skip(4);
        Assert.Equal("    x-axis \"Maximum Output\" [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]", reader.NextLine());
        Assert.Equal("    y-axis \"Weight\"", reader.NextLine());
        Assert.Equal("    bar [3, 3, 3, 3, 3, 4, 4, 5, 6, 6]", reader.NextLine());
    }

    private static LinesReader Render(DocBarChartAttribute attribute)
    {
        var collector = Collect.ValuesOf<string>();
        TheScribe.GetArtery = _ => collector;
        var fragment = TheArchivist.ToFragment(attribute, typeof(DocBarChartTests), typeof(DocBarChartTests));
        var explanation = new Explanation("Chart", [fragment]);

        TheScribe.Print("ignored.md", new Book([new Page(explanation, "Chart.md")], [], []));

        return LinesReader.FromStringList([.. collector.Values]);
    }

    private static class CurveData
    {
        internal static readonly (int MaximumOutput, int Weight)[] WeightCurve =
        [
            (1, 3),
            (2, 3),
            (3, 3),
            (4, 3),
            (5, 3),
            (6, 4),
            (7, 4),
            (8, 5),
            (9, 6),
            (10, 6)
        ];
    }
}
