namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DocLink(string name, Type target, string section = "") : Attribute
{
    public string Name { get; } = name;
    public Type Target { get; } = target;
    public string Section { get; } = section;
}
