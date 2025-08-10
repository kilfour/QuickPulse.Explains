namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class DocReplaceAttribute : Attribute
{
    public string From { get; }
    public string To { get; }
    public DocReplaceAttribute(string from, string to)
    {
        From = from; To = to;
    }
}
