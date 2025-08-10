namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DocFileHeaderAttribute : Attribute
{
    public string Header { get; }

    public DocFileHeaderAttribute(string header)
    {
        Header = header;
    }
}
