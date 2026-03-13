using QuickPulse.Explains.Exceptions;

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
        if (stillSame && currentNamespace.Length > rootNamespace.Length)
        {
            differsAt = rootNamespace.Length;
            stillSame = false;
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

    public static string GetFileContents(string filePath, string filename, int skipLines, int? numberOfLines)
    {
        var codeFile = Path.Combine(Path.GetDirectoryName(filePath)!, filename);
        if (!File.Exists(codeFile)) throw new DocCodeFileNotFoundException(codeFile);
        var lines = File.ReadAllLines(codeFile).Skip(skipLines);
        if (numberOfLines.HasValue)
            lines = lines.Take(numberOfLines.Value);
        return string.Join(Environment.NewLine, lines);
    }

    public static string GetRawFileContents(string filePath, string filename)
    {
        var codeFile = Path.Combine(Path.GetDirectoryName(filePath)!, filename);
        if (!File.Exists(codeFile)) throw new DocRawFileNotFoundException(codeFile);
        return File.ReadAllText(codeFile);
    }
}