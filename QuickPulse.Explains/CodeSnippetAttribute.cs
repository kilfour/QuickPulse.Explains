using System.Runtime.CompilerServices;

namespace QuickPulse.Explains;

/// <summary>
/// Marks a method whose body should be extracted and embedded as a code snippet in generated documentation.
/// Unlike <see cref="CodeExampleAttribute"/>, this includes only the method body,
/// omitting the signature and declaration details.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CodeSnippetAttribute(
    [CallerFilePath] string file = "",
    [CallerLineNumber] int line = 0) : Attribute
{
    public string File { get; } = file;
    public int Line { get; } = line;
}

