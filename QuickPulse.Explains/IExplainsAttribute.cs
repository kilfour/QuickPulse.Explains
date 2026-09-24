namespace QuickPulse.Explains;

/// <summary>
/// Composes documentation and code attributes into one reusable attribute.
/// </summary>
public interface IExplainsAttribute
{
    /// <summary>
    /// Returns existing documentation and code attributes. Documentation attributes retain
    /// their relative order, as do code transformations. Forward the composite's caller file
    /// and line to any CodeExampleAttribute or CodeSnippetAttribute returned here.
    /// Nested composite expansion is not supported.
    /// </summary>
    IEnumerable<Attribute> Expand();
}
