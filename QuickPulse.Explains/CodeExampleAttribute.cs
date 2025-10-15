using System.Runtime.CompilerServices;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class CodeExampleAttribute(
    [CallerFilePath] string file = "",
    [CallerLineNumber] int line = 0) : Attribute
{
    public string File { get; } = file;
    public int Line { get; } = line;
}
