using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Embeds a literal code block directly into the generated documentation.
/// The code is included as-is and rendered using the specified syntax highlighting language.
/// Useful for showing small examples or fragments that are not extracted from source files.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocCodeAttribute(string content, string language = "csharp") : DocFragmentAttribute
{
    public string Code { get; } = content;
    public string Language { get; } = language;
}