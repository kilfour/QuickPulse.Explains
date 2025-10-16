using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Declares a reference-style link label for use in documentation.
/// Generates a link definition mapping the given name to the target [DocFile], optionally to a section anchor.
/// Use the label in Markdown like [Text][Name] to reference the generated definition.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DocLinkAttribute(string name, Type target, string section = "") : DocFragmentAttribute
{
    public string Name { get; } = name;
    public Type Target { get; } = target;
    public string Section { get; } = section;
}
