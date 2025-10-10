using QuickPulse.Explains.Monastery.Fragments;

namespace QuickPulse.Explains.Monastery;

public record Explanation(string HeaderText, IReadOnlyList<Fragment> Fragments);
