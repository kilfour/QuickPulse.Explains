namespace QuickPulse.Explains.Monastery.Writings;

public record Book(
    IReadOnlyCollection<Page> Pages,
    IReadOnlyCollection<Inclusion> Inclusions,
    IReadOnlyCollection<Example> Examples)
        : Reference(Inclusions, Examples);
