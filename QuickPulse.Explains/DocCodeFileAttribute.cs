using System.Runtime.CompilerServices;
using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

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
