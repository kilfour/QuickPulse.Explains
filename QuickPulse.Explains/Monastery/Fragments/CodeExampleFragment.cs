namespace QuickPulse.Explains.Monastery.Fragments;

public record CodeExampleFragment(
    string Name,
    string Language,
    Type? SourceType = null) : Fragment;

