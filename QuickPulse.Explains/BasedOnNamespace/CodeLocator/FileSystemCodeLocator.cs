namespace QuickPulse.Explains.BasedOnNamespace.CodeLocator;

public class FileSystemCodeLocator : ICodeLocator
{
    public IEnumerable<string> ReadAfter(string file, int line) =>
        File.ReadLines(file).Skip(line).Select(x => x + Environment.NewLine);
}
