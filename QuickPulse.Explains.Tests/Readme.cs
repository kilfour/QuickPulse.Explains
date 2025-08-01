

using QuickPulse.Explains.Deprecated;

namespace QuickPulse.Explains.Tests;

[Doc(Order = "0", Caption = "QuickPulse.Explains", Content =
@"`QuickPulse.Explains` is a lightweight documentation generator for C# projects,
designed to turn `[Doc]`-annotated test classes and methods into clean, structured Markdown files.
It leverages the `QuickPulse` library for declarative flow composition and supports
both single-file and multi-file generation with namespace-based filtering.
")]
//[DocFile]
[DocFileHeader("QuickPulse.Explains")]
public class ReadMe
{
    [Fact(Skip = "migrate doc to new way of doing things")]
    [DocContent(
@"`QuickPulse.Explains` is a lightweight documentation generator for C# projects,
designed to turn `[Doc]`-annotated test classes and methods into clean, structured Markdown files.
It leverages the `QuickPulse` library for declarative flow composition and supports
both single-file and multi-file generation with namespace-based filtering.
")]
    public void GenerateReadme()
    {
        // new Document().ToFiles([new("README.md",
        //     [ "QuickPulse.Explains.Tests"
        //     , "QuickPulse.Explains.Tests.Text"
        //     ])], typeof(Readme).Assembly);

        //Explain.This<DocumentedThing>("thing.md");
        Explain.This<ReadMe>("temp.md");
        //Explain.These<ReadMe>("TheDoc");
        // Explain.These<DocumentedThing>("Temp");
    }
}
