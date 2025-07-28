namespace QuickPulse.Explains.BasedOnNamespace;

public record Book(
    IReadOnlyCollection<Page> Pages,
    IReadOnlyCollection<Inclusion> Includes);
