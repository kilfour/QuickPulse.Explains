namespace QuickPulse.Explains.Monastery.CodeLocator;

public interface ICodeLocator
{
    IEnumerable<string> ReadAfter(string file, int line);
}
