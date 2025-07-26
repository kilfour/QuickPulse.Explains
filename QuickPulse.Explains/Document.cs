using QuickPulse.Arteries;
using QuickPulse;
using System.Reflection;
using QuickPulse.Explains.Bolts;

namespace QuickPulse.Explains;

public class Document
{
    public Func<string, IArtery> GetArtery = filename => new WriteDataToFile(filename).ClearFile();

    public void ToFile(string filename, Assembly assembly)
    {
        PulseIt(GetArtery(filename), SearchAssembly.GetDocAttributes(assembly));
    }

    public void ToFiles(IEnumerable<DocFileInfo> files, Assembly assembly)
    {
        var allDocs = SearchAssembly.GetDocEntries(assembly).ToList();
        foreach (var file in files)
        {
            var matchingDocs = allDocs
                .Where(d =>
                {
                    var nsToCheck = (d.Member as Type)?.Namespace ?? d.Member.DeclaringType?.Namespace;
                    return nsToCheck is not null &&
                        file.Namespaces.Any(ns => nsToCheck.StartsWith(ns));
                })
                .Select(d => d.Attribute)
                .ToList();

            if (matchingDocs.Any())
            {
                var artery = GetArtery(file.Filename);
                PulseIt(artery, matchingDocs);
            }
        }
    }

    private void PulseIt(IArtery artery, List<DocAttribute> doc)
        => Signal.From(Render.AsMarkdown)
            .SetArtery(artery)
            .Pulse(doc);
}

