using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests.Text;

[DocFile]
[DocFileHeader("LinesReader: Sequential Line Navigation")]
[DocContent(
@"`LinesReader` is a lightweight utility intended for use in tests. It provides simple,
sequential line-by-line reading over string content.

**Creating a Reader**

**- From text input (newline-separated):**
```csharp
var reader = LinesReader.FromText(aString);
```

**- From a list of strings:**
```csharp
var reader = LinesReader.FromStringList(new[] { ""line1"", ""line2"" });
```

**Basic usage:**
```csharp
var reader = LinesReader.FromText(someText);
Assert.Equal(""This is the first line of a two line text."", reader.NextLine());
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
LinesReader.FromStringList([""one"", ""two""]).AsAssertsToLogFile();
```
*Produces output like:*
```csharp
        Assert.Equal(""one"", reader.NextLine());
        Assert.Equal(""two"", reader.NextLine());
        Assert.True(reader.EndOfContent());
```")]
public class LinesReaderTests
{
    [Fact]
    public void NextLine_ReadsLinesInOrder()
    {
        var reader = LinesReader.FromText("line1\r\nline2\r\nline3");
        Assert.Equal("line1", reader.NextLine());
        Assert.Equal("line2", reader.NextLine());
        Assert.Equal("line3", reader.NextLine());
    }

    [Fact]
    public void EndOfContent_ReturnsTrueAtEnd()
    {
        var reader = LinesReader.FromText("onlyline");
        reader.NextLine();
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void EndOfContent_Throws_Before_End()
    {
        var reader = LinesReader.FromText("a\r\nb");
        reader.NextLine();
        var ex = Assert.Throws<QuickPulse.Instruments.ComputerSaysNo>(() => reader.EndOfContent());
        Assert.Equal("Not end of content: 'b'.", ex.Message);
    }

    [Fact]
    public void Skip_SkipsLine()
    {
        var reader = LinesReader.FromText("first\r\nskipme\r\nthird");
        reader.NextLine();
        reader.Skip();
        Assert.Equal("third", reader.NextLine());
    }

    [Fact]
    public void SkipMany_SkipsCorrectNumberOfLines()
    {
        var reader = LinesReader.FromText("a\r\nb\r\nc\r\nd");
        reader.Skip(2);
        Assert.Equal("c", reader.NextLine());
    }

    [Fact]
    public void FromText_EmptyString_ReturnsEmptyLine()
    {
        var reader = LinesReader.FromText("");
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void NextLine_Throws_WhenReadingPastEnd()
    {
        var reader = LinesReader.FromText("only");
        reader.NextLine();
        var ex = Assert.Throws<QuickPulse.Instruments.ComputerSaysNo>(reader.NextLine);
        Assert.Equal("Attempted to read past the end of content.", ex.Message);
    }

    [Fact]
    public void FromStringList_Works()
    {
        var reader = LinesReader.FromStringList(["one", "two"]);
        Assert.Equal("one", reader.NextLine());
        Assert.Equal("two", reader.NextLine());
    }
}
