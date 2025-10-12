namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CodeRemoveAttribute : CodeReplaceAttribute
{
    public CodeRemoveAttribute(string from)
        : base(from, "") { }
}
