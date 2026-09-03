using QuickPulse.Arteries;
using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Monastery.Fragments.Tables;
using QuickPulse.Explains.Monastery.Writings;
using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests.Tables;

public class TableRenderingTests
{
    [Fact]
    public void Escapes_pipes_backslashes_and_line_breaks_in_every_cell()
    {
        var table = new TableFragment(
            ["Name|Alias", @"Path\Pattern"],
            [
                new RowFragment(
                    new FirstCellFragment("first|second\r\nthird", "Target.md", "#target"),
                    [new CellFragment("C:\\temp|archive\nnext")])
            ]);
        var collector = Collect.ValuesOf<string>();
        TheScribe.GetArtery = _ => collector;
        var explanation = new Explanation("Table", [table]);

        TheScribe.Print(
            "ignored.md",
            new Book([new Page(explanation, "Table.md")], [], []));

        var reader = LinesReader.FromStringList([.. collector.Values]);
        Assert.Equal("# Table", reader.NextLine());
        Assert.Equal("| Name\\|Alias| Path\\\\Pattern |", reader.NextLine());
        Assert.Equal("| -| - |", reader.NextLine());
        Assert.Equal(
            "| [first\\|second<br>third](#target)| C:\\\\temp\\|archive<br>next |",
            reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}
