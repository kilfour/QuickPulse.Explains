using System.Globalization;
using QuickPulse;
using System.Reflection;

namespace QuickPulse.Explains.Deprecated.Bolts;

public static class SearchAssembly
{
    public static List<DocEntry> GetDocEntries(Assembly assembly)
    {
        var types = assembly.GetTypes();

        var typeAttributes =
            types
                .SelectMany(t => t
                    .GetCustomAttributes<DocAttribute>(false)
                    .Select(attr => new DocEntry(attr, t)));

        var methodAttributes =
            types
                .SelectMany(t => t.GetMethods())
                .SelectMany(m => m
                    .GetCustomAttributes<DocAttribute>(false)
                    .Select(attr => new DocEntry(attr, m)));

        return typeAttributes.Concat(methodAttributes)
            .OrderBy(e => ParseOrder(e.Attribute.Order), new LexicalFloatArrayComparer())
            .ToList();
    }

    public static List<DocAttribute> GetDocAttributes(Assembly assembly)
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
            typeattributes.Concat(methodattributes)
                .OrderBy(a => ParseOrder(a.Order), new LexicalFloatArrayComparer()).ToList();
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
}
