using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Declares a reference-style link label for use in documentation.
/// Generates a link definition mapping the given name to the target [DocFile], optionally to a section anchor.
/// Use the label in Markdown like [Text][Name] to reference the generated definition.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DocLinkAttribute(Type target, string name = "", string section = "") : DocFragmentAttribute
{
    public string Name { get; } = string.IsNullOrWhiteSpace(name) ? target.Name : name;
    public Type Target { get; } = target;
    public string Section { get; } = section;
}
