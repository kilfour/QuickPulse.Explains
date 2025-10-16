namespace QuickPulse.Explains;

/// <summary>
/// Marks a code region or pattern to be removed when embedding code in documentation.
/// Functions as a shorthand for <see cref="CodeReplaceAttribute"/> with an empty replacement,
/// allowing cleanup of boilerplate or irrelevant lines in extracted examples.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class CodeRemoveAttribute(string from) : CodeReplaceAttribute(from, "")
{
}
