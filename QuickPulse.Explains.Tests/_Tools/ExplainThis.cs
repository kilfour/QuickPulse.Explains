using System.Reflection;

namespace QuickPulse.Explains.Tests._Tools;

public static class ExplainThis
{
    public static void Invoke(Type dynamicType, string filename)
    {
        // hardcoded for class Explain and static method This<T>(string)
        var method = typeof(Explain)
            .GetMethod("This", BindingFlags.Public | BindingFlags.Static);

        if (method == null)
            throw new InvalidOperationException("Method 'This' not found on Explain.");

        var closed = method.MakeGenericMethod(dynamicType);
        closed.Invoke(null, new object[] { filename });
    }
}
