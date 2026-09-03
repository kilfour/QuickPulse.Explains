using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Adds a Markdown table containing documentation classes from a child namespace.
/// Each matching <see cref="DocFileAttribute"/> class becomes a row whose cells are populated
/// from its <see cref="DocColumnAttribute"/> declarations in the specified column order.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocTableAttribute(string namespaceName, params string[] columns) : DocFragmentAttribute
{
    public string NamespaceName { get; } = namespaceName;
    public string[] Columns { get; } = columns;
}
