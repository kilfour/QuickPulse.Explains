namespace QuickPulse.Explains.Monastery;

public record SeperatePage(
    Page Page,
    IReadOnlyCollection<Inclusion> Inclusions,
    IReadOnlyCollection<Example> Examples)
        : Reference(Inclusions, Examples);
