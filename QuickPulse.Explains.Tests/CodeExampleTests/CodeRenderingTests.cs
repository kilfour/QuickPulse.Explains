using QuickPulse.Arteries;
using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Monastery.Fragments;
using QuickPulse.Explains.Monastery.Writings;

namespace QuickPulse.Explains.Tests.CodeExampleTests;

public class CodeRenderingTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Embedded_backticks_cannot_close_the_outer_code_block(bool example)
    {
        const string code = "```markdown\n````\n```";
        Fragment fragment = example
            ? new CodeExampleFragment("example", "markdown")
            : new CodeFragment(code, "markdown");
        var collector = Collect.ValuesOf<string>();
        var previous = TheScribe.GetArtery;
        try
        {
            TheScribe.GetArtery = _ => collector;
            TheScribe.Print("ignored.md", new Book(
                [new Page(new Explanation("Code", [fragment]), "Code.md")],
                [], [new Example("example", code)]));

            Assert.Equal(["# Code", "`````markdown", code, "`````"], collector.Values);
        }
        finally
        {
            TheScribe.GetArtery = previous;
        }
    }
}
