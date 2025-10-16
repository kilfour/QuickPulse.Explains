using QuickPulse.Explains.Monastery.Fragments;

namespace QuickPulse.Explains.Monastery.Writings;

public record Explanation(string HeaderText, IReadOnlyList<Fragment> Fragments);
