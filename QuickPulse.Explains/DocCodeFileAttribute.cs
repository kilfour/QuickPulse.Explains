using System.Runtime.CompilerServices;
using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocCodeFileAttribute(string filename, string language = "", [CallerFilePath] string path = "") : DocFragmentAttribute
{
    public string Filename { get; } = filename;
    public string Language { get; } = language;
    public string Path { get; } = path;
}
