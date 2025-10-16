namespace QuickPulse.Explains;

/// <summary>
/// Marks a class as a documentation source.
/// Depending on the entry point used by <see cref="Explain.This"/>, <see cref="Explain.OnlyThis"/>, or <see cref="Explain.These"/>,
/// the annotated class can either produce a standalone Markdown file or be included within a larger composed document.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class DocFileAttribute : Attribute { }

