using QuickPulse.Explains.BasedOnNamespace;

namespace QuickPulse.Explains.TestModels;

[Doc("1", "Test Class", "This is a class-level doc.")]
[DocFile]
public class DocumentedThing
{
    [Doc("1-1", "Test Method", "This is a method-level doc.")]
    public void DoStuff() { }

    [DocHeader("should be 2")]
    [DocContent("Some Content")]
    [DocInclude(typeof(SomeOtherStuff))]
    public void NoMethodNameCaptions() { }
}
