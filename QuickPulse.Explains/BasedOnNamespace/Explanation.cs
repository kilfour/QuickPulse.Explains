namespace QuickPulse.Explains.BasedOnNamespace;

public abstract record Fragment { }

public record HeaderFragment(string Header, int Level) : Fragment;
public record ContentFragment(string Content) : Fragment;
public record InclusionFragment(Type Included) : Fragment;

public record Explanation(string HeaderText, IReadOnlyList<Fragment> Fragments);
