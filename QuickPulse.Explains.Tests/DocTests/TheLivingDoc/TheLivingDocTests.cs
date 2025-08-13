using System.Reflection;
using System.Reflection.Emit;
using QuickPulse.Arteries;
using QuickPulse.Explains.BasedOnNamespace;
using QuickPulse.Explains.Tests._Tools;
using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests.DocTests.TheLivingDoc;

[DocFile]
[DocContent(
@"**QuickPulse.Explains** supports embedding source code directly into generated documentation through
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
This ensures your documentation always reflects the current, runnable source without manual copy-paste.")]
public class TheLivingDocTests
{
    [Fact]
    [DocContent("`CodeExample` extracts a method or class for use as an example to be included later.")]
    public void CodeExample()
    {
        var module = DynamicModuleBuilder.Create();
        var includedType = DynamicTypeBuilder.Create("SomeOtherClass", module)
            .WithVoidMethod<CodeExampleAttribute>("MyMethod", "code.cs", 0)
            .Build();
        var type = DynamicTypeBuilder.Create("TheLivingDoc", module)
           .WithClassAttribute<DocFileAttribute>()
           .WithClassAttribute<DocExampleAttribute>(includedType, "MyMethod", "csharp")
           .Build();
        var collector = new TheCollector<string>();
        TheScribe.GetArtery = a => collector;
        var codeLocator = new CodeLocatorMock()
            .Add("code.cs", 0,
                "public static class Demo {",
                "    public static void Run() {",
                "        Console.WriteLine(\"Hi\");",
                "    }",
                "}",
                "not interested");
        TheArchivist.GetCodeLocator = () => codeLocator;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromStringList([.. collector.TheExhibit.SelectMany(a => a.Split(Environment.NewLine))]);
        Assert.Equal(
            [
                "# The Living Doc",
                "```csharp",
                "public static class Demo {",
                "    public static void Run() {",
                "        Console.WriteLine(\"Hi\");",
                "    }",
                "}",
                "```",
            ], reader.NextLines(8));
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocContent(@"`CodeSnippet` is similar to CodeExample, only it extracts the body of a method, ignoring the signature.")]
    public void CodeSnippet()
    {
    }

    [Fact]
    [DocContent(
@"`DocCodeFile` pulls in an entire file.
The path to the file needs to be specified and is relative to the class containing the attribute.")]
    public void CodeFile()
    {

    }
}