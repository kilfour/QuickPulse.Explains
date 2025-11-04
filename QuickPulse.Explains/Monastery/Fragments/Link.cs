namespace QuickPulse.Explains.Monastery.Fragments;

public class LinkFragment(string Name, string Location) : Fragment
{
    public string Name { get; } = Name;
    public string Location { get; } = Location;
}