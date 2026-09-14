# Composing documentation attributes

Implement `IDocAttribute` on a custom attribute to bundle existing documentation
attributes. Apply it to a documentation class or method alongside ordinary attributes.

```csharp
using QuickPulse.Explains;
using QuickPulse.Explains.Abstractions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class ExamplesAttribute(Type source, params string[] members)
    : Attribute, IDocAttribute
{
    public IEnumerable<DocFragmentAttribute> Expand()
    {
        yield return new DocHeaderAttribute("Examples");
        foreach (var member in members)
            yield return new DocExampleAttribute(source, member);
    }
}

[DocFile]
[DocContent("Some examples:")]
[Examples(typeof(Sample), nameof(Sample.Answer))]
public class SampleDocumentation;

public class Sample
{
    [CodeExample]
    public static int Answer() => 42;
}
```

Expansion occupies the composite attribute's position and preserves the order of
the returned attributes. Existing link behavior still applies: links appear after
other fragments. Expanded includes support nested includes and cycle detection;
expanded examples use the normal source extraction pipeline.

Each attribute occurrence is expanded once per book composition, including when its
declaring type is both a page and an include. Return existing `DocFragmentAttribute`
instances; nested composite expansion and custom rendering are not supported.
`DocFile` and `DocFileHeader` remain separate class annotations.

## Composing code attributes

Implement `ICodeAttribute` and return `CodeAttribute` instances to bundle
`CodeExample`, `CodeSnippet`, `CodeReplace`, and `CodeRemove`. `CodeFormat` is also
supported. Replacements keep their expansion order alongside direct attributes;
formatters run after replacements, as usual.

When including an example or snippet marker, capture the caller location in your
custom attribute's constructor and forward it explicitly. Otherwise the marker
would point to the implementation of `Expand()` instead of the annotated source.

```csharp
using System.Runtime.CompilerServices;
using QuickPulse.Explains;
using QuickPulse.Explains.Abstractions;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class CleanExampleAttribute(
    [CallerFilePath] string file = "",
    [CallerLineNumber] int line = 0) : Attribute, ICodeAttribute
{
    public IEnumerable<CodeAttribute> Expand()
    {
        yield return new CodeExampleAttribute(file, line);
        yield return new CodeRemoveAttribute("// implementation detail");
        yield return new CodeReplaceAttribute("InternalName", "PublicName");
    }
}

public class ExampleSource
{
    [CleanExample]
    public static int Answer() => 42;
}
```

Reference the source with `[DocExample(typeof(ExampleSource), nameof(ExampleSource.Answer))]`
as usual. For body-only extraction, yield `new CodeSnippetAttribute(file, line)`
instead. A composite may also return only transformations and be applied alongside
a direct `[CodeExample]` or `[CodeSnippet]`.

Custom code attribute names discovered on the source are recognized during
extraction and stripped from the displayed example. Unrelated attributes remain.
Expansion is materialized and reused for references to the same source type within
a book. Multiple example/snippet markers for the same reference remain ambiguous
and produce an error. Nested composite expansion is not supported.
