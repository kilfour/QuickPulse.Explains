using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocContentAttribute : DocFragmentAttribute
{
    public string Content { get; }

    public DocContentAttribute(string content)
    {
        Content = content;
    }
}
