using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// TODO
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DocColumnAttribute(string columnName, string content) : Attribute
{
    public string ColumnName { get; } = columnName;
    public string Content { get; } = content;
}