using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DocLinkAttribute(string name, Type target, string section = "") : DocFragmentAttribute
{
    public string Name { get; } = name;
    public Type Target { get; } = target;
    public string Section { get; } = section;
}
