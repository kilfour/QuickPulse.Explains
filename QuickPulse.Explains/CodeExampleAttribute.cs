using System.Runtime.CompilerServices;

namespace QuickPulse.Explains;

/// <summary>
/// Marks a class or method as a source for code extraction.
/// Used by documentation generators to embed the exact code from the
/// original file and line location where this attribute is applied.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class CodeExampleAttribute(
    [CallerFilePath] string file = "",
    [CallerLineNumber] int line = 0) : Attribute
{
    public string File { get; } = file;
    public int Line { get; } = line;
}
