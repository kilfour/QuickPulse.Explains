using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Embeds a code example extracted from a specified type or method into the generated documentation.
/// When only a type is provided, the entire class is included. When a method name is given,
/// only that method’s code is extracted and rendered with syntax highlighting.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocExampleAttribute(Type type, string methodName = "", string language = "csharp") : DocFragmentAttribute
{
    public string Name { get; } = methodName == "" ? type.FullName! : type.FullName + "." + methodName;
    public string Language { get; } = language;
}

