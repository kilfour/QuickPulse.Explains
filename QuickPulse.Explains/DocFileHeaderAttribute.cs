namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DocFileHeaderAttribute(string header) : Attribute
{
    public string Header { get; } = header;
}
