using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Includes the documentation content of another [DocFile]-annotated class at the current position.
/// Useful for composing larger documents by inlining related sections or shared examples
/// without duplicating their source definitions.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocIncludeAttribute(Type includedDoc) : DocFragmentAttribute
{
    public Type Included { get; } = includedDoc;
}