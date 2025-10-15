namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CodeRemoveAttribute(string from) : CodeReplaceAttribute(from, "")
{
}
