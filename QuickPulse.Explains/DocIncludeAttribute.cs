using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocIncludeAttribute(Type includedDoc) : DocFragmentAttribute
{
    public Type Included { get; } = includedDoc;
}
