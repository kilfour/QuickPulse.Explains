namespace QuickPulse.Explains.Tests.Tables;

[DocFile]
public class TableSpike
{
    public record Columns
    {
        public const string First = "First Column";
        public const string Second = "Second Column";

    }

    [Fact]
    //[Fact(Skip = "explicit")]
    [DocTable(nameof(Entries), Columns.First, Columns.Second)]
    public void Content()
    {
        Explain.These<TableSpike>("./QuickPulse.Explains.Tests/Tables/");
    }
}