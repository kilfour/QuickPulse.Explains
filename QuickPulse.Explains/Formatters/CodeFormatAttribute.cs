namespace QuickPulse.Explains.Formatters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class CodeFormatAttribute : Attribute
{
    public Type FormatterType { get; }

    public CodeFormatAttribute(Type formatterType, string? methodName = null, params object?[] args)
    {
        FormatterType = formatterType;
    }
}
