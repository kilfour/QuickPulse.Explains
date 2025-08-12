using System.Runtime.CompilerServices;
using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class CodeFileAttribute : DocFragmentAttribute
{
    public string Filename { get; }
    public string Language { get; }
    public string Path { get; }

    public CodeFileAttribute(string filename, string language = "", [CallerFilePath] string path = "")
    {
        Language = language;
        Path = path;
        Filename = filename;
    }
}
