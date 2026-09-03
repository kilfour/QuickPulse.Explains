using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Adds a Mermaid XY bar chart to the generated documentation.
/// The chart data is read from a static field or property containing numeric (x, y) tuples.
/// The Y-axis range is omitted unless supplied explicitly.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocBarChartAttribute(
    string dataMember,
    string title,
    string xAxis,
    string yAxis) : DocFragmentAttribute
{
    public DocBarChartAttribute(
        Type dataSourceType,
        string dataMember,
        string title,
        string xAxis,
        string yAxis)
        : this(dataMember, title, xAxis, yAxis)
    {
        DataSourceType = dataSourceType;
    }

    public DocBarChartAttribute(
        string dataMember,
        string title,
        string xAxis,
        string yAxis,
        double yAxisMinimum,
        double yAxisMaximum)
        : this(dataMember, title, xAxis, yAxis)
    {
        YAxisMinimum = yAxisMinimum;
        YAxisMaximum = yAxisMaximum;
    }

    public DocBarChartAttribute(
        Type dataSourceType,
        string dataMember,
        string title,
        string xAxis,
        string yAxis,
        double yAxisMinimum,
        double yAxisMaximum)
        : this(dataSourceType, dataMember, title, xAxis, yAxis)
    {
        YAxisMinimum = yAxisMinimum;
        YAxisMaximum = yAxisMaximum;
    }

    public Type? DataSourceType { get; }
    public string DataMember { get; } = dataMember;
    public string Title { get; } = title;
    public string XAxis { get; } = xAxis;
    public string YAxis { get; } = yAxis;
    public double? YAxisMinimum { get; }
    public double? YAxisMaximum { get; }
}
