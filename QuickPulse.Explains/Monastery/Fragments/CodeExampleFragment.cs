namespace QuickPulse.Explains.Monastery.Fragments;

public class CodeExampleFragment(string Name, string Language) : Fragment
{
    public string Name { get; } = Name;
    public string Language { get; } = Language;
}