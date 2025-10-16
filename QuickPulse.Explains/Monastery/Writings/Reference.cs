namespace QuickPulse.Explains.Monastery.Writings;

public abstract record Reference(
    IReadOnlyCollection<Inclusion> Inclusions,
    IReadOnlyCollection<Example> Examples);