using System.Reflection;
using System.Reflection.Emit;
using QuickPulse.Arteries;
using QuickPulse.Explains.BasedOnNamespace;
using QuickPulse.Explains.Tests._Tools;
using QuickPulse.Explains.Text;


namespace QuickPulse.Explains.Tests.DocTests.GettingStarted;

[DocFile]
[DocContent("This example shows how to add documentation for a simple file.")]
public class ASimpleFile
{
    [Fact]
    [DocHeader("Adding a `DocFile` Attribute")]
    [DocContent(
@"By putting this attribute on the class, and calling `Explain<T>.This(...)` a file will be created in the
specified location, relative to the solution root.
```csharp
[DocFile]
public class ASimpleFile { }

Explain.This<ASimpleFile>(""HowTo.md"");
```
At this point the output will contain only a header, which is derived from the class name:
```markdown
# A Simple File
```")]
    public void DocFile_attribute()
    {
        var type = DynamicTypeBuilder.Create("ASimpleFile")
            .WithClassAttribute<DocFileAttribute>()
            .Build();

        var holden = TheString.Catcher();
        TheScribe.GetArtery = a => holden;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromText(holden.Whispers());
        Assert.Equal("# A Simple File", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocHeader("The `DocFileHeader` Attribute")]
    [DocContent(
@"It is also possible to supply a *custom* header by applying a `DocFileHeader` attribute to the class definition.
```csharp
[DocFile]
[DocFileHeader(""My Custom Header"")]
public class ASimpleFile { }
```
```markdown
# My Custom Header
```
**Note:** Both `DocFile` and `DocFileHeader` are restricted to class definitions.
")]
    public void DocFile_attribute_with_custom_header()
    {
        var type = DynamicTypeBuilder.Create("ASimpleFile")
            .WithClassAttribute<DocFileAttribute>()
            .WithClassAttribute<DocFileHeaderAttribute>("My Custom Header")
            .Build();

        var holden = TheString.Catcher();
        TheScribe.GetArtery = a => holden;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromText(holden.Whispers());
        Assert.Equal("# My Custom Header", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocHeader("Paragraphs with `DocContent`")]
    [DocContent(
@"In order to add some text, rendered as is to the output use the `DocContent` attribute.
```csharp
[DocFile]
[DocContent(""This example shows how to add documentation for a simple file."")]
public class ASimpleFile { }

```
Renders as:
```markdown
# A Simple File
This example shows how to add documentation for a simple file.
```
> Do you see where this is going ?
")]
    public void DocContent_attribute_on_class()
    {
        var type = DynamicTypeBuilder.Create("ASimpleFile")
            .WithClassAttribute<DocFileAttribute>()
            .WithClassAttribute<DocContentAttribute>("my content")
            .Build();

        var collector = new TheCollector<string>();
        TheScribe.GetArtery = a => collector;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromStringList([.. collector.TheExhibit]);
        Assert.Equal("# A Simple File", reader.NextLine());
        Assert.Equal("my content  ", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocContent(
@"`DocContent` is also a valid attribute on methods.
```csharp
[DocFile]
[DocContent(""This example shows how to add documentation for a simple file."")]
public class ASimpleFile 
{ 
    [Fact]
    [DocContent(""By putting this attribute on the class, ..."")] 
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
")]
    public void DocContent_attribute_on_method()
    {
        var type = DynamicTypeBuilder.Create("ASimpleFile")
            .WithClassAttribute<DocFileAttribute>()
            .WithClassAttribute<DocContentAttribute>("my class content")
            .WithVoidMethod<DocContentAttribute>("MyMethod", "my method content")
            .Build();

        var collector = new TheCollector<string>();
        TheScribe.GetArtery = a => collector;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromStringList([.. collector.TheExhibit]);
        Assert.Equal("# A Simple File", reader.NextLine());
        Assert.Equal("my class content  ", reader.NextLine());
        Assert.Equal("my method content  ", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocHeader("`DocHeader`")]
    [DocContent(
@"`DocHeader` is only valid on methods. It renders the way you would think, as a header, one level higher than the DocFileHeader.
So:
```csharp
[DocFile]
[DocContent(""This example shows how to add documentation for a simple file."")]
public class ASimpleFile 
{ 
    [Fact]
    [DocHeader(""Adding a `DocFile` Attribute"")]
    [DocContent(""By putting this attribute on the class, ..."")] 
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
")]
    public void DocHeader_attribute_on_method()
    {
        var type = DynamicTypeBuilder.Create("ASimpleFile")
            .WithClassAttribute<DocFileAttribute>()
            .WithVoidMethod<DocHeaderAttribute>("MyMethod", "my method header", 0)
            .Build();

        var collector = new TheCollector<string>();
        TheScribe.GetArtery = a => collector;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromStringList([.. collector.TheExhibit]);
        Assert.Equal("# A Simple File", reader.NextLine());
        Assert.Equal("## my method header", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocContent(
@"If, occasionally, you need to have a header of a different level, for a small sub section or so,
there is an overload of `DocHeader` which takes an `int` parameter. 
This parameter gets added to the context aware current level of the header.

So, this:
```csharp
[DocFile]
public class ASimpleFile 
{ 
    [Fact]
    [DocHeader(""Level Two"")]
    [DocHeader(""Level Three"", 1)] 
    public void MyMethod() { }
}
```
Renders as:
```markdown
# A Simple File
## Level Two
### Level Three
```
")]
    public void DocHeader_attribute_on_method_with_level()
    {
        var type = DynamicTypeBuilder.Create("ASimpleFile")
            .WithClassAttribute<DocFileAttribute>()
            .WithVoidMethod<DocHeaderAttribute>("MyMethod", "my method header", 1)
            .Build();

        var collector = new TheCollector<string>();
        TheScribe.GetArtery = a => collector;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromStringList([.. collector.TheExhibit]);
        Assert.Equal("# A Simple File", reader.NextLine());
        Assert.Equal("### my method header", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocContent(
@"You could of course just put *'## Level Two'* etc. inside the previously shown `DocContent` attribute and be done with it.  
That is how the previous version of this lib worked.  

The advantage of using the `DocHeader` attribute is that it's level is based on context.

You can if you have multiple *Doc-decorated* classes, render them all to one file.
In which case header level is based on namespace *depth*.  

You can also render them as seperate files in which case each file will be rendered similarely to as what has been shown here.  
And then there's also includes ... .  
")]
    [DocHeader("So What About These `DocInclude`'s ?")]
    [DocContent(
@"Well, let me show you.

Given this:
```csharp
[DocFile]
public class ASimpleFile 
{  
    [DocHeader(""Header From My Method"")]
    [DocInclude(typeof(SomeOtherClass))] 
    public void MyMethod() { }
}

public class SomeOtherClass 
{ 
    [DocHeader(""Header From SomeOtherClass Method"")] 
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
")]
    public void DocIncludes()
    {
        var asmName = new AssemblyName("DocIncludeTestAssembly");
        var asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        var module = asm.DefineDynamicModule("Main");
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            if (args.Name.StartsWith(asm.FullName!))
                return asm;
            return null;
        };

        var includedType = DynamicTypeBuilder.Create("SomeOtherClass", module)
            .WithVoidMethod<DocHeaderAttribute>("MyMethod", "Header From SomeOtherClass Method", 0)
            .Build();

        var type = DynamicTypeBuilder.Create("ASimpleFile", module)
            .WithClassAttribute<DocFileAttribute>()
            .WithVoidMethod<DocIncludeAttribute>("MyMethod", includedType)
            .Build();

        var collector = new TheCollector<string>();
        TheScribe.GetArtery = a => collector;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromStringList([.. collector.TheExhibit]);
        Assert.Equal("# A Simple File", reader.NextLine());
        Assert.Equal("## Some Other Class", reader.NextLine());
        Assert.Equal("### Header From SomeOtherClass Method", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}
