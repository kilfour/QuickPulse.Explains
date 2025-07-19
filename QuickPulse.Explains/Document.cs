using System.Globalization;
using QuickPulse.Bolts;
using QuickPulse.Arteries;
using QuickPulse;
using System.Reflection;

namespace QuickPulse.Explains;

public class Document
{
    public Func<string, IArtery> GetArtery = filename => new WriteDataToFile(filename).ClearFile();

    public void ToFile(string filename, Assembly assembly)
    {
        var artery = GetArtery(filename);
        var attrs = GetDocAttributes(assembly).ToList();
        Signal.From(RenderMarkdown).SetArtery(artery).Pulse(attrs);
    }

    public void ToFiles(IEnumerable<DocFileInfo> files, Assembly assembly)
    {
        var allDocs = GetDocEntries(assembly).ToList();

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
                Signal.From(RenderMarkdown).SetArtery(artery).Pulse(matchingDocs);
            }
        }
    }

    private static readonly Flow<DocAttribute> RenderMarkdown =
        from doc in Pulse.Start<DocAttribute>()
        let headingLevel = doc.Order.Split('-').Length
        from rcaption in Pulse
            .NoOp(/* ---------------- Render Caption  ---------------- */ )
        let caption = doc.Caption
        let hasCaption = !string.IsNullOrEmpty(doc.Caption)
        let headingMarker = new string('#', headingLevel)
        let captionLine = $"{headingMarker} {caption}"
        from _t2 in Pulse.TraceIf(hasCaption, captionLine)
        from rcontent in Pulse
            .NoOp(/* ---------------- Render content  ---------------- */ )
        let content = doc.Content
        let hasContent = !string.IsNullOrEmpty(content)
        from _t3 in Pulse.TraceIf(hasContent, content, "")
        from end in Pulse
            .NoOp(/* ---------------- End of content  ---------------- */ )
        select doc;

    private static IOrderedEnumerable<DocEntry> GetDocEntries(Assembly assembly)
    {
        var types = assembly.GetTypes();

        var typeAttributes =
            types
                .SelectMany(t => t
                    .GetCustomAttributes(typeof(DocAttribute), false)
                    .Cast<DocAttribute>()
                    .Select(attr => new DocEntry(attr, t)));

        var methodAttributes =
            types
                .SelectMany(t => t.GetMethods())
                .SelectMany(m => m
                    .GetCustomAttributes(typeof(DocAttribute), false)
                    .Cast<DocAttribute>()
                    .Select(attr => new DocEntry(attr, m)));

        return typeAttributes
            .Concat(methodAttributes)
            .OrderBy(e => ParseOrder(e.Attribute.Order), new LexicalFloatArrayComparer());
    }

    private IOrderedEnumerable<DocAttribute> GetDocAttributes(Assembly assembly)
    {
        var typeattributes =
            assembly
                .GetTypes()
                .SelectMany(t => t.GetCustomAttributes<DocAttribute>());

        var methodattributes =
            assembly
                .GetTypes()
                .SelectMany(t => t.GetMethods())
                .SelectMany(t => t.GetCustomAttributes<DocAttribute>());
        return
            typeattributes
                .Union(methodattributes)
                .OrderBy(attr => ParseOrder(attr.Order), new LexicalFloatArrayComparer());
    }

    private class LexicalFloatArrayComparer : IComparer<float[]>
    {
        public int Compare(float[]? x, float[]? y)
        {
            if (x == null || y == null) return Comparer<float[]>.Default.Compare(x, y);
            var len = Math.Max(x.Length, y.Length);
            for (int i = 0; i < len; i++)
            {
                var a = i < x.Length ? x[i] : 0f;
                var b = i < y.Length ? y[i] : 0f;
                var cmp = a.CompareTo(b);
                if (cmp != 0) return cmp;
            }
            return 0;
        }
    }

    private static float[] ParseOrder(string order)
    {
        return [.. order
            .Split('-')
            .Select(part =>
                float.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out var f)
                    ? f
                    : throw new FormatException($"Invalid order segment: '{part}'"))];
    }

    private record DocEntry(DocAttribute Attribute, MemberInfo Member);
}