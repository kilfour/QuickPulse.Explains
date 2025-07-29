namespace QuickPulse.Explains.BasedOnNamespace;

public static class ThePathfinder
{
    public static string NamespaceRelativeFilename(Type root, Type current)
    {
        var rootNamespace = root.Namespace ?? "";
        var currentNamespace = current.Namespace ?? "";
        var relativeNamespace = currentNamespace.StartsWith(rootNamespace)
            ? currentNamespace.Substring(rootNamespace.Length).TrimStart('.')
            : currentNamespace;
        var path = Path.Combine(relativeNamespace.Split('.', StringSplitOptions.RemoveEmptyEntries));
        return Path.Combine(path, current.Name + ".md");
    }
}