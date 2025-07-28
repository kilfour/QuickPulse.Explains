using _TestModel;
using QuickPulse.Arteries;


namespace QuickPulse.Explains.Tests;

public class DocumentTests
{
    [Fact]
    [Doc(Order = "0-1-2", Caption = "Single Markdown File Generation", Content =
@"To write all collected documentation into one Markdown file:

```csharp
var doc = new Document();
doc.ToFile(""docs.md"", typeof(CalculatorTests).Assembly);
```

This emits headers and content based on the `[Doc]` attributes, using heading depth inferred from the order string.")]
    public void ToFile_EmitsExpectedMarkdown()
    {
        var collector = new TheCollector<string>();
        var document = new Document
        {
            GetArtery = _ => collector
        };

        document.ToFile("ignored.md", typeof(DocumentedThing).Assembly);

        Assert.Contains(collector.TheExhibit, l => l.StartsWith("# Test Class"));
        Assert.Contains(collector.TheExhibit, l => l.Contains("This is a class-level doc."));
        Assert.Contains(collector.TheExhibit, l => l.StartsWith("## Test Method"));
        Assert.Contains(collector.TheExhibit, l => l.Contains("This is a method-level doc."));
    }

    [Fact]
    [Doc(Order = "0-1-3", Caption = "Multi-File Generation with Namespace Filtering", Content =
@"Group output by namespace:
```csharp
var doc = new Document();
doc.ToFiles([
    new DocFileInfo(""domain.md"", [""MyApp.Domain""]),
    new DocFileInfo(""tests.md"", [""MyApp.Tests""])
], Assembly.GetExecutingAssembly());
```
This allows documentation to be split by concern or target audience.

## Installation

QuickPulse is available on NuGet:

```bash
Install-Package QuickPulse.Explains
```

Or via the .NET CLI:

```bash
dotnet add package QuickPulse.Explains
```")]
    public void ToFiles_RespectsNamespaceFiltering()
    {
        var collector = new TheCollector<string>();
        var document = new Document
        {
            GetArtery = _ => collector
        };

        var files = new[]
        {
            new DocFileInfo("filtered.md", ["_TestModel"])
        };

        document.ToFiles(files, typeof(DocumentedThing).Assembly);

        Assert.Contains(collector.TheExhibit, l => l.Contains("Test Class"));
        Assert.Contains(collector.TheExhibit, l => l.Contains("Test Method"));
    }

    [Fact]
    public void ToFiles_SkipsUnmatchedNamespaces()
    {
        var collector = new TheCollector<string>();
        var document = new Document
        {
            GetArtery = _ => collector
        };

        var files = new[]
        {
            new DocFileInfo("ignored.md", new List<string> { "Non.Existent.Namespace" })
        };

        document.ToFiles(files, typeof(DocumentedThing).Assembly);

        Assert.Empty(collector.TheExhibit);
    }
}
