namespace QuickPulse.Explains.Monastery.Fragments;

public class CodeFragment(string Code, string Language) : Fragment
{
    public string Code { get; } = Code;
    public string Language { get; } = Language;
}