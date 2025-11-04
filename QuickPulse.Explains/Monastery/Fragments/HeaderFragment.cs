namespace QuickPulse.Explains.Monastery.Fragments;

public class HeaderFragment(string Header, int Level) : Fragment
{
    public string Header { get; } = Header;
    public int Level { get; } = Level;
}