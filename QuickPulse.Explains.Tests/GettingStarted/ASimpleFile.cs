using QuickPulse.Arteries;
using QuickPulse.Explains.BasedOnNamespace;
using QuickPulse.Explains.Tests._Tools;
using QuickPulse.Explains.Text;


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
    [Fact]
    public void AddingADocFile()
    {
        var type = TypeBuilder.Create("MyType")
            .WithClassAttribute<DocFileAttribute>()
            .Build();

        var holden = TheString.Catcher();
        TheScribe.GetArtery = a => holden;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromText(holden.Whispers());
        Assert.Equal("# My Type", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}
