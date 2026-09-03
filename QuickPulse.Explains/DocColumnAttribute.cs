namespace QuickPulse.Explains;

/// <summary>
/// Defines the content of a named table column for a <see cref="DocFileAttribute"/> class.
/// The annotated class becomes a row when selected by a <see cref="DocTableAttribute"/>,
/// with each column matched by name.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DocColumnAttribute(string columnName, string content) : Attribute
{
    public string ColumnName { get; } = columnName;
    public string Content { get; } = content;
}
