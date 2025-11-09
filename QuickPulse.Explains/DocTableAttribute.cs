using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// TODO
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocTableAttribute(string namespaceName, params string[] columns) : DocFragmentAttribute
{
    public string NamespaceName { get; } = namespaceName;
    public string[] Columns { get; } = columns;
}
