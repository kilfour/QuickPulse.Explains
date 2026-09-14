using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>Composes existing code extraction and transformation attributes.</summary>
public interface ICodeAttribute
{
    /// <summary>
    /// Returns existing code attributes in order. Forward the composite's caller file
    /// and line to any CodeExampleAttribute or CodeSnippetAttribute returned here.
    /// Nested composite expansion is not supported.
    /// </summary>
    IEnumerable<CodeAttribute> Expand();
}
