using System.Runtime.CompilerServices;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class DocExampleAttribute : Attribute
{
    public string File { get; }
    public int Line { get; }

    public DocExampleAttribute(
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        File = file;
        Line = line;
    }
}
