# QuickPulse.Explains
`QuickPulse.Explains` is a lightweight documentation generator for C# projects,
designed to turn `[Doc]`-annotated test classes and methods into clean, structured Markdown files.
It leverages the `QuickPulse` library for declarative flow composition and supports
both single-file and multi-file generation with namespace-based filtering.

## Attribute-Based Documentation
Apply the `[Doc]` attribute to your test classes and methods:

```csharp
[Doc(Order = "1", Caption = "The Calculator", Content = "This class calculates totals.")]
public class CalculatorTests
{
    [Fact]
    [Doc(Order = "1-1", Caption = "Empty Cart", Content = "An empty cart always totals to 0.")]
    public void EmptyCart_ReturnsZero() => ...
}
```
The `Order` defines hierarchy (`1`, `1-1`, etc.), `Caption` becomes the header, and `Content` is the body text.
### Single Markdown File Generation
To write all collected documentation into one Markdown file:

```csharp
var doc = new Document();
doc.ToFile("docs.md", typeof(CalculatorTests).Assembly);
```

This emits headers and content based on the `[Doc]` attributes, using heading depth inferred from the order string.
### Multi-File Generation with Namespace Filtering
Group output by namespace:
```csharp
var doc = new Document();
doc.ToFiles([
    new DocFileInfo("domain.md", ["MyApp.Domain"]),
    new DocFileInfo("tests.md", ["MyApp.Tests"])
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
```
## Bits and Alices
As I use aforementioned method off documenting stuff in all of my test projects, I will most likely end up putting
all of the various tools which are duplicated across multiple projects right now inside of this little box over here.
### LinesReader: Sequential Line Navigation
`LinesReader` is a lightweight utility intended for use in tests. It provides simple,
sequential line-by-line reading over string content.

**Creating a Reader**

**- From text input (newline-separated):**
```csharp
var reader = LinesReader.FromText(aString);
```

**- From a list of strings:**
```csharp
var reader = LinesReader.FromStringList(new[] { "line1", "line2" });
```

**Basic usage:**
```csharp
var reader = LinesReader.FromText(someText);
Assert.Equal("This is the first line of a two line text.", reader.NextLine());
reader.Skip();
Assert.True(reader.EndOfContent());
```

**Error Handling:**

LinesReader throws exceptions if:
- You call `NextLine()` or `Skip()` past the end of the content.
- You use the reader before it's initialized.
- You call `EndOfContent()` when there are still lines to read.

#### AsAssertsToLogFile
`.AsAssertsToLogFile()` outputs the contents of the reader as xUnit `Assert.Equal(...)` statements,
writing them to a log file using QuickPulse's default location:
> `.quickpulse\quick-pulse-{unique}.log`.
**Example:**
```csharp
LinesReader.FromStringList(["one", "two"]).AsAssertsToLogFile();
```
*Produces output like:*
```csharp
        Assert.Equal("one", reader.NextLine());
        Assert.Equal("two", reader.NextLine());
        Assert.True(reader.EndOfContent());
```
