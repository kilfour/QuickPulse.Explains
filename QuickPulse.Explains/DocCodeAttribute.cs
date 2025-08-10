using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocCodeAttribute : DocFragmentAttribute
{
    public string Code { get; }
    public string Language { get; }

    public DocCodeAttribute(string content, string language = "csharp")
    {
        Code = content;
        Language = language;
    }
}
