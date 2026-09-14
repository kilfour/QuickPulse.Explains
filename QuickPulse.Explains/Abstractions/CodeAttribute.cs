namespace QuickPulse.Explains.Abstractions;

/// <summary>Base type for attributes used to extract and transform code examples.</summary>
public abstract class CodeAttribute : Attribute
{
    internal string[] CompositionAttributeNames { get; set; } = [];
}
