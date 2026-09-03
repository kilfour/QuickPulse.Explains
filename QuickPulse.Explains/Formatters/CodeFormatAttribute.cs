namespace QuickPulse.Explains.Formatters;

/// <summary>
/// Applies a custom <see cref="ICodeFormatter"/> to source code extracted for generated documentation.
/// The formatter type is instantiated when the code example or snippet is composed.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CodeFormatAttribute(Type formatterType) : Attribute
{
    public Type FormatterType { get; } = formatterType;
}
