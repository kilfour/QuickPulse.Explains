using QuickPulse.Explains.BasedOnNamespace.CodeLocator;

namespace QuickPulse.Explains.Tests._Tools;

public sealed class CodeLocatorMock : ICodeLocator
{
    private readonly Dictionary<(string file, int line), string[]> _data = new();

    public CodeLocatorMock Add(string file, int line, params string[] lines)
    {
        _data[(file, line)] = lines;
        return this;
    }

    public IEnumerable<string> ReadAfter(string file, int line) =>
        _data.TryGetValue((file, line), out var lines) ? lines.Select(x => x + Environment.NewLine) : Array.Empty<string>();
}