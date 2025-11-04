namespace QuickPulse.Explains.Monastery.Fragments;

public class InclusionFragment(Type Included) : Fragment
{
    public Type Included { get; } = Included;
}