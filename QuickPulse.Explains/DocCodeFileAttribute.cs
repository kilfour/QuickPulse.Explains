using System.Runtime.CompilerServices;
using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Includes the contents of an external source file as a formatted code block in the generated documentation.
/// The path is resolved relative to the file where the attribute is declared,
/// and an optional number of lines can be skipped from the top of the file.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocCodeFileAttribute(
    string filename,
    string language = "",
    int skipLines = 0,
    [CallerFilePath] string path = "") : DocFragmentAttribute
{
    public string Filename { get; } = filename;
    public string Language { get; } = language;
    public int SkipLines { get; } = skipLines;
    public string Path { get; } = path;
}