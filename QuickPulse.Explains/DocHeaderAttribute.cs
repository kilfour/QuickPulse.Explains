using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Inserts a Markdown header into the generated documentation at the current context level.
/// The optional level offset allows fine-tuning of the header depth relative to the surrounding structure,
/// enabling nested sections or subheaders within a [DocFile] or method documentation block.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DocHeaderAttribute(string header, int level = 0) : DocFragmentAttribute
{
    public string Header { get; } = header;
    public int Level { get; } = level;
}