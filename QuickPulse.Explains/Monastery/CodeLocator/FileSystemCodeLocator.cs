namespace QuickPulse.Explains.Monastery.CodeLocator;

public class FileSystemCodeLocator : ICodeLocator
{
    public IEnumerable<string> ReadAfter(string file, int line) =>
        File.ReadLines(file).Skip(line);
}
