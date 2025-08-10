using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DocHeaderAttribute : DocFragmentAttribute
{
    public string Header { get; }
    public int Level { get; }

    public DocHeaderAttribute(string header, int level = 0)
    {
        Header = header;
        Level = level;
    }
}
