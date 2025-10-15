using System.Runtime.CompilerServices;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CodeSnippetAttribute(
    [CallerFilePath] string file = "",
    [CallerLineNumber] int line = 0) : Attribute
{
    public string File { get; } = file;
    public int Line { get; } = line;
}
