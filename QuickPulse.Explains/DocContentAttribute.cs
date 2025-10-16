using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Adds a plain text or Markdown paragraph to the generated documentation.
/// The content is rendered exactly as provided and can be used for explanations,
/// notes, or narrative sections alongside code examples and headers.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocContentAttribute(string content) : DocFragmentAttribute
{
    public string Content { get; } = content;
}
