namespace QuickPulse.Explains.Monastery.Fragments;

public record BarChartFragment(
    string Title,
    string XAxis,
    string YAxis,
    IReadOnlyList<string> XValues,
    IReadOnlyList<string> YValues,
    double? YAxisMinimum,
    double? YAxisMaximum) : Fragment;
