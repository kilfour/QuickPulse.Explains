using _TestModel;

namespace QuickPulse.Explains.Tests;

[Doc(Order = "0", Caption = "QuickPulse.Explains", Content =
@"`QuickPulse.Explains` is a lightweight documentation generator for C# projects,
designed to turn `[Doc]`-annotated test classes and methods into clean, structured Markdown files.
It leverages the `QuickPulse` library for declarative flow composition and supports
both single-file and multi-file generation with namespace-based filtering.
")]
public class Readme
{
    [Fact]
    public void GenerateReadme()
    {
        // new Document().ToFiles([new("README.md",
        //     [ "QuickPulse.Explains.Tests"
        //     , "QuickPulse.Explains.Tests.Text"
        //     ])], typeof(Readme).Assembly);

        //Explain.This<Readme>("temp.md");
        Explain.These<AutoDocumentedThing>("Docs");
    }
}
