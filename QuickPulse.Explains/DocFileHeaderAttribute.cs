namespace QuickPulse.Explains;

/// <summary>
/// Specifies a custom top-level header for a [DocFile] class.
/// Overrides the default header, which would otherwise be derived from the class name.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DocFileHeaderAttribute(string header) : Attribute
{
    public string Header { get; } = header;
}