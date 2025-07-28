using QuickPulse.Arteries;
using QuickPulse;
using System.Reflection;
using QuickPulse.Bolts;
using QuickPulse.Instruments;

namespace QuickPulse.Explains;

public record DocFileEntry(DocFileAttribute Attribute, Type Type);

public static class Explain
{
    public static Func<string, IArtery> GetArtery { get; set; } =
        filename => new WriteDataToFile(filename).ClearFile();

    public static void This<T>(string filename)
    {
        var docFiles = GetDocFiles(typeof(T).Assembly.GetTypes());
        Signal.From(File)
            .SetArtery(WriteData.ToNewFile(filename))
            .Pulse(docFiles);
    }

    public static void These<T>(string path)
    {
        if (string.IsNullOrEmpty(path))
            ComputerSays.No($"Path: '{path}' is not valid.");

        var types = typeof(T).Assembly.GetTypes();
        var docFiles = GetDocFiles(types)
            .Select(a =>
            {
                var rel = Path.Combine(GetNamespaceRelativePath(typeof(T), a), a.Name + ".md");
                return (Filename: Path.Combine(path, rel), Relative: rel, DocFile: a);
            })
            .ToList();

        foreach (var (Filename, Relative, DocFile) in docFiles)
        {
            Signal.From(File)
                .SetArtery(WriteData.ToNewFile(Filename))
                .Pulse(DocFile);
        }

        var entries = docFiles.Select(a => new ToCEntry(GetHeaderText(a.DocFile.Name), a.Relative));

        Signal.From(ToC)
            .SetArtery(WriteData.ToNewFile(Path.Combine(path, "ToC.md")))
            .Pulse(entries);
    }


    private static string GetNamespaceRelativePath(Type root, Type current)
    {
        var rootNs = root.Namespace ?? "";
        var currentNs = current.Namespace ?? "";

        // Strip the root namespace prefix
        var relativeNs = rootNs.Length <= currentNs.Length
            ? currentNs[rootNs.Length..]
            : currentNs;

        // Turn dots into path separators
        var segments = relativeNs.Split('.', StringSplitOptions.RemoveEmptyEntries);
        return Path.Combine(segments);
    }

    public record ToCEntry(string Text, string Path);

    private static readonly Flow<ToCEntry> ToC =
        from input in Pulse.Start<ToCEntry>()
        from _ in Pulse.Trace($"- [{input.Text}]({input.Path})")
        select input;

    private static IOrderedEnumerable<Type> GetDocFiles(Type[] types)
        => types.Where(a => a.GetCustomAttributes<DocFileAttribute>(false).Any()).OrderBy(a => a.FullName);

    private static IEnumerable<DocMethodAttribute> GetDocMethods(Type type)
        => type.GetMethods().SelectMany(a => a.GetCustomAttributes<DocMethodAttribute>(false));

    private static readonly Flow<string> MarkDownHeader =
         from text in Pulse.Start<string>()
         from level in Pulse.Gather<int>()
         let headingMarker = new string('#', level.Value)
         let header = $"{headingMarker} {text}"
         from _ in Pulse.Trace(header)
         select text;

    private static readonly Flow<DocHeaderAttribute> Header =
         from attr in Pulse.Start<DocHeaderAttribute>()
         from _ in Pulse.ToFlow(MarkDownHeader, attr.Header)
         select attr;

    private static readonly Flow<DocContentAttribute> Content =
         from attr in Pulse.Start<DocContentAttribute>()
         from _ in Pulse.Trace(attr.Content)
         select attr;

    private static readonly Flow<DocMethodAttribute> Method =
         from attr in Pulse.Start<DocMethodAttribute>()
         from _h in Pulse.ToFlowIf(attr is DocHeaderAttribute, Header, () => (DocHeaderAttribute)attr)
         from _c in Pulse.ToFlowIf(attr is DocContentAttribute, Content, () => (DocContentAttribute)attr)
         select attr;

    private static int GetLevel(Box<string> root, string path)
        => path[root.Value.Length..].Split(".").Length;

    private static string GetHeaderText(string typeName)
    {
        var result = typeName;
        if (typeName.Contains('_'))
            result = string.Join("", typeName.Split("_").Skip(1));
        result = result.Replace("_", " ");
        return result.Aggregate("", (a, b) => char.IsUpper(b) ? a + " " + b : a + b).Trim();
    }

    private static readonly Flow<Type> File =
        from type in Pulse.Start<Type>()
        let path = type.Namespace ?? ""
        from root in Pulse.Gather(path)
        from level in Pulse.Gather(1)
        from _ in Pulse.Scoped<int>(a => GetLevel(root, path),
            from _ in Pulse.ToFlow(MarkDownHeader, GetHeaderText(type.Name))
            from s in Pulse.Scoped<int>(a => a + 1, Pulse.ToFlow(Method, GetDocMethods(type)))
            select Unit.Instance)
        select type;
}