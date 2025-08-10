using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class DocIncludeAttribute : DocFragmentAttribute
{
    public Type Included { get; }

    public DocIncludeAttribute(Type includedDoc)
    {
        Included = includedDoc;
    }
}
