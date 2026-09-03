namespace QuickPulse.Explains.Formatters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CodeFormatAttribute(Type formatterType) : Attribute
{
    public Type FormatterType { get; } = formatterType;
}
