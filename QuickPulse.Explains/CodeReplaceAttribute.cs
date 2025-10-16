namespace QuickPulse.Explains;

/// <summary>
/// Defines a text replacement to apply when embedding source code into documentation.
/// Allows transformation or cleanup of extracted code by replacing specific fragments
/// before rendering them in the generated output.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CodeReplaceAttribute(string from, string to) : Attribute
{
    public string From { get; } = from;
    public string To { get; } = to;
}
