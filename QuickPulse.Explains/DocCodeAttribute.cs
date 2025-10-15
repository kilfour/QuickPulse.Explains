using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocCodeAttribute(string content, string language = "csharp") : DocFragmentAttribute
{
    public string Code { get; } = content;
    public string Language { get; } = language;
}