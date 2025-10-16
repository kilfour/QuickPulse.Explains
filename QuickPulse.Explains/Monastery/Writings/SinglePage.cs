using QuickPulse.Explains.Monastery.Writings;

namespace QuickPulse.Explains.Monastery;

public record SinglePage(
    Page Page,
    IReadOnlyCollection<Inclusion> Inclusions,
    IReadOnlyCollection<Example> Examples)
        : Reference(Inclusions, Examples);
