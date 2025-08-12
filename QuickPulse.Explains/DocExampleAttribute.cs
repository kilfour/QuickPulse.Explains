using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocExampleAttribute : DocFragmentAttribute
{
    public string Name { get; }

    public DocExampleAttribute(Type type, string methodName = "")
    {
        Name = methodName == "" ? type.FullName! : type.FullName + "." + methodName;
    }
}
