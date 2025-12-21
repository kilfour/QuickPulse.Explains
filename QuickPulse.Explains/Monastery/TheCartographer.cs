namespace QuickPulse.Explains.Monastery;

public static class TheCartographer
{
    public static string ChartPath(Type root, Type current)
    {
        var rootNamespace = root.Namespace ?? "";
        var currentNamespace = current.Namespace ?? "";
        var relativeNamespace = currentNamespace.StartsWith(rootNamespace)
            ? currentNamespace[rootNamespace.Length..].TrimStart('.')
            : currentNamespace;
        var path = Path.Combine(relativeNamespace.Split('.', StringSplitOptions.RemoveEmptyEntries));
        return Path.Combine(path, current.Name + ".md").Replace("\\", "/");
    }

    public static string ChartLinkPath(Type root, Type current)
    {
        var rootNamespace = root.Namespace == null ? [] : root.Namespace.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var currentNamespace = current.Namespace == null ? [] : current.Namespace.Split('.', StringSplitOptions.RemoveEmptyEntries);
        bool stillSame = true;
        int differsAt = 0;
        var result = new List<string>();
        for (int i = 0; i < rootNamespace.Length; i++)
        {
            if (stillSame && i >= currentNamespace.Length)
            {
                differsAt = i;
                stillSame = false;
            }
            if (stillSame && rootNamespace[i] == currentNamespace[i])
                continue;
            if (stillSame) differsAt = i;
            stillSame = false;
            result.Add("..");
        }
        if (!stillSame)
        {
            for (int i = differsAt; i < currentNamespace.Length; i++)
            {
                result.Add(currentNamespace[i]);
            }
        }
        var path = Path.Combine([.. result]);
        return Path.Combine(path, current.Name + ".md").Replace("\\", "/");
    }

    public static string GetFileContents(string filePath, string filename, int skipLines)
    {
        var codeFile = Path.Combine(Path.GetDirectoryName(filePath)!, filename);
        return string.Join(Environment.NewLine, File.ReadAllLines(codeFile).Skip(skipLines));
    }
}