using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocExampleAttribute(Type type, string methodName = "", string language = "csharp") : DocFragmentAttribute
{
    public string Name { get; } = methodName == "" ? type.FullName! : type.FullName + "." + methodName;
    public string Language { get; } = language;
}
