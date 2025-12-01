using System.Runtime.CompilerServices;
using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Includes the contents of an external file as is in the generated documentation.
/// The path is resolved relative to the file where the attribute is declared.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocRawFileAttribute(
    string filename,
    [CallerFilePath] string path = "") : DocFragmentAttribute
{
    public string Filename { get; } = filename;
    public string Path { get; } = path;
}

