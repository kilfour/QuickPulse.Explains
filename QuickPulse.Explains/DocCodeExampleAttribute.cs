using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocCodeExampleAttribute : DocFragmentAttribute
{
    public string Name { get; }

    public DocCodeExampleAttribute(Type type, string methodName, params string[] replacements)
    {
        Name = type.FullName + "." + methodName;
    }
}
