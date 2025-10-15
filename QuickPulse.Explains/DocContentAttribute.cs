using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocContentAttribute(string content) : DocFragmentAttribute
{
    public string Content { get; } = content;
}
