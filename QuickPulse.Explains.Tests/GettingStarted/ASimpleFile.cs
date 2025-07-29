using System.Reflection;
using QuickPulse.Explains.BasedOnNamespace;
using QuickPulse.Explains.Tests._Tools;

namespace QuickPulse.Explains.Tests.GettingStarted;

// Generates a page with a heading "A Simple File"
[DocFile]
[DocContent("Introductionary text for 'A Simple File'.")]
public class ASimpleFile
{
    // This will render as a subsection (##) under "A Simple File"
    // Header: "Adding a Doc File"
    // Content: two paragraphs
    [DocHeader("Adding a Doc File")]
    [DocContent("This example shows how to add documentation for a simple file.")]
    [DocContent("Multiple DocContent attributes will be combined as separate paragraphs.")]
    public void AddingADocFile()
    {
        var type = TypeBuilder.Create()
            .WithClassAttribute<DocFileAttribute>()
            .Build();
    }

    // This will create another subsection
    [DocHeader("Second Example")]
    [DocContent("forget about the dots.")]
    public void AnotherExample()
    {
    }
}
