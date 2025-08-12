using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocExampleAttribute : DocFragmentAttribute
{
    public string Name { get; }
    public string Language { get; }

    public DocExampleAttribute(Type type, string methodName = "", string language = "csharp")
    {
        Name = methodName == "" ? type.FullName! : type.FullName + "." + methodName;
        Language = language;
    }
}
