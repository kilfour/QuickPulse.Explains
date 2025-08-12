namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class CodeReplaceAttribute : Attribute
{
    public string From { get; }
    public string To { get; }
    public CodeReplaceAttribute(string from, string to)
    {
        From = from; To = to;
    }
}
