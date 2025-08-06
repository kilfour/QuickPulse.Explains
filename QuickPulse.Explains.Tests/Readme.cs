
namespace QuickPulse.Explains.Tests;


[DocFile]
[DocFileHeader("QuickPulse.Explains")]
public class ReadMe
{
    [Fact]
    [DocContent(
@"`QuickPulse.Explains` is a lightweight documentation generator for C# projects,
designed to turn `[Doc]`-annotated test classes and methods into clean, structured Markdown files.
It leverages the `QuickPulse` library for declarative flow composition and supports
both single-file and multi-file generation with namespace-based filtering.
")]
    public void GenerateReadme()
    {
        Explain.This<ReadMe>("README.md");
        //Explain.These<ReadMe>("TheDoc");
    }
}
