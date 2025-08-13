namespace QuickPulse.Explains.BasedOnNamespace.CodeLocator;

public interface ICodeLocator
{
    IEnumerable<string> ReadAfter(string file, int line);
}
