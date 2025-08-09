namespace QuickPulse.Explains.BasedOnNamespace;

public record SeperatePage(
    Page Page,
    IReadOnlyCollection<Inclusion> Inclusions,
    IReadOnlyCollection<Example> Examples);
