using System.Runtime.CompilerServices;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class CodeExampleAttribute : Attribute
{
    public string File { get; }
    public int Line { get; }

    public CodeExampleAttribute(
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        File = file;
        Line = line;
    }
}
