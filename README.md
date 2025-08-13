# QuickPulse.Explains
`QuickPulse.Explains` is a lightweight documentation generator for C# projects,
designed to turn `[Doc]`-annotated test classes and methods into clean, structured Markdown files.
It leverages the `QuickPulse` library for declarative flow composition and supports
both single-file and multi-file generation with namespace-based filtering.
  
## A Simple File
This example shows how to add documentation for a simple file.  
### Adding a `DocFile` Attribute
By putting this attribute on the class, and calling `Explain<T>.This(...)` a file will be created in the
specified location, relative to the solution root.
```csharp
[DocFile]
public class ASimpleFile { }

Explain.This<ASimpleFile>("HowTo.md");
```
At this point the output will contain only a header, which is derived from the class name:
```markdown
# A Simple File
```  
### The `DocFileHeader` Attribute
It is also possible to supply a *custom* header by applying a `DocFileHeader` attribute to the class definition.
```csharp
[DocFile]
[DocFileHeader("My Custom Header")]
public class ASimpleFile { }
```
```markdown
# My Custom Header
```
**Note:** Both `DocFile` and `DocFileHeader` are restricted to class definitions.
  
### Paragraphs with `DocContent`
In order to add some text, rendered as is to the output use the `DocContent` attribute.
```csharp
[DocFile]
[DocContent("This example shows how to add documentation for a simple file.")]
public class ASimpleFile { }

```
Renders as:
```markdown
# A Simple File
This example shows how to add documentation for a simple file.
```
> Do you see where this is going ?
  
`DocContent` is also a valid attribute on methods.
```csharp
[DocFile]
[DocContent("This example shows how to add documentation for a simple file.")]
public class ASimpleFile 
{ 
    [Fact]
    [DocContent("By putting this attribute on the class, ...")] 
    public void DocFile_Attribute_Test()
    {
        // test code here
    }
}
```
Renders as:
```markdown
# A Simple File
This example shows how to add documentation for a simple file.
By putting this attribute on the class, ...
```
  
### `DocHeader`
`DocHeader` is only valid on methods. It renders the way you would think, as a header, one level higher than the DocFileHeader.
So:
```csharp
[DocFile]
[DocContent("This example shows how to add documentation for a simple file.")]
public class ASimpleFile 
{ 
    [Fact]
    [DocHeader("Adding a `DocFile` Attribute")]
    [DocContent("By putting this attribute on the class, ...")] 
    public void DocFile_Attribute_Test()
    {
        // test code here
    }
}
```
Renders as:
```markdown
# A Simple File
This example shows how to add documentation for a simple file.
## Adding a `DocFile` Attribute
By putting this attribute on the class, ...
```
  
If, occasionally, you need to have a header of a different level, for a small sub section or so,
there is an overload of `DocHeader` which takes an `int` parameter. 
This parameter gets added to the context aware current level of the header.

So, this:
```csharp
[DocFile]
public class ASimpleFile 
{ 
    [Fact]
    [DocHeader("Level Two")]
    [DocHeader("Level Three", 1)] 
    public void MyMethod() { }
}
```
Renders as:
```markdown
# A Simple File
## Level Two
### Level Three
```
  
You could of course just put *'## Level Two'* etc. inside the previously shown `DocContent` attribute and be done with it.  
That is how the previous version of this lib worked.  

The advantage of using the `DocHeader` attribute is that it's level is based on context.

You can if you have multiple *Doc-decorated* classes, render them all to one file.
In which case header level is based on namespace *depth*.  

You can also render them as seperate files in which case each file will be rendered similarely to as what has been shown here.  
And then there's also includes ... .  
  
### So What About These `DocInclude`'s ?
Well, let me show you.

Given this:
```csharp
[DocFile]
public class ASimpleFile 
{  
    [DocHeader("Header From My Method")]
    [DocInclude(typeof(SomeOtherClass))] 
    public void MyMethod() { }
}

public class SomeOtherClass 
{ 
    [DocHeader("Header From SomeOtherClass Method")] 
    public void MyOtherMethod() { }
}
```
It renders as:
```markdown
# A Simple File
## Header From My Method
## Some Other Class
### Header From SomeOtherClass Method
```
  
## The Living Doc
**QuickPulse.Explains** supports embedding source code directly into generated documentation through
a set of Code... attributes.  
These attributes don't create documentation content themselves, instead,
they reference code from your project so it can be displayed alongside explanations, headers, and other doc elements. 

In order to include the examples one needs to apply a `DocExample` attribute which will inject the referenced example in the markdown doc.  

Example:
```csharp
[DocFile]
[DocExample(typeof(Bar))]
public class Foo
{
    [CodeExample]
    private class Bar
    {
        public int Method() { return 42; }
    }
}
```
Renders as:
# Foo
```csharp
private class Bar
{
    public int Method() { return 42; }
}
```
**Important:** The code extraction only works for methods with a *block body*,
it currently fails miserably when confronted with *expression bodied* methods.

All extracted code is formatted and syntax-highlighted automatically, preserving indentation and spacing.
This ensures your documentation always reflects the current, runnable source without manual copy-paste.  
`CodeExample` extracts a method or class for use as an example to be included later.  
`CodeSnippet` is similar to CodeExample, only it extracts the body of a method, ignoring the signature.  
`DocCodeFile` pulls in an entire file.
The path to the file needs to be specified and is relative to the class containing the attribute.  
## Talking About The Tools
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
- You call `NextLine()`, `NextLines()` or `Skip()` past the end of the content.
- You use the reader before it's initialized.
- You call `EndOfContent()` when there are still lines to read.

#### SkipToLineContaining
Utility function that allows you to skip ahead to the first line containing a specific string.

#### ReadLines(int howMany)
Returns an array of strings, starting at current index and reading `howMany` lines into said array.


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
