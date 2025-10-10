namespace QuickPulse.Explains.Monastery;

public abstract record Reference(
    IReadOnlyCollection<Inclusion> Inclusions,
    IReadOnlyCollection<Example> Examples);