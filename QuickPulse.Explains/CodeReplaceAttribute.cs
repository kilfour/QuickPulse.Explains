namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CodeReplaceAttribute(string from, string to) : Attribute
{
    public string From { get; } = from; public string To { get; } = to;
}
