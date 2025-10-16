namespace QuickPulse.Explains.Monastery;

public static class TheCartographer
{
    public static string ChartPath(Type root, Type current)
    {
        var rootNamespace = root.Namespace ?? "";
        var currentNamespace = current.Namespace ?? "";
        var relativeNamespace = currentNamespace.StartsWith(rootNamespace)
            ? currentNamespace.Substring(rootNamespace.Length).TrimStart('.')
            : currentNamespace;
        var path = Path.Combine(relativeNamespace.Split('.', StringSplitOptions.RemoveEmptyEntries));
        return Path.Combine(path, current.Name + ".md").Replace("\\", "/");
    }

    public static string GetFileContents(string filePath, string filename, int skipLines)
    {
        var codeFile = Path.Combine(Path.GetDirectoryName(filePath)!, filename);
        return string.Join("", File.ReadAllLines(codeFile).Skip(skipLines));
    }
}