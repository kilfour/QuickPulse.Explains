using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class DocHeaderAttribute(string header, int level = 0) : DocFragmentAttribute
{
    public string Header { get; } = header;
    public int Level { get; } = level;
}
