using QuickPulse.Explains.Abstractions;

namespace QuickPulse.Explains;

/// <summary>
/// Composes existing documentation attributes into a reusable class or method attribute.
/// </summary>
public interface IDocAttribute
{
    /// <summary>
    /// Returns documentation attributes in their intended order. Expansion is evaluated once
    /// per attribute occurrence during book composition. Return existing fragment attributes,
    /// rather than further composite attributes.
    /// </summary>
    IEnumerable<DocFragmentAttribute> Expand();
}
