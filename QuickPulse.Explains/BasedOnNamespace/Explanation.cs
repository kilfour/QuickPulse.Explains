using QuickPulse.Explains.BasedOnNamespace.Fragments;

namespace QuickPulse.Explains.BasedOnNamespace;

public record Explanation(string HeaderText, IReadOnlyList<Fragment> Fragments);
