namespace QuickPulse.Explains.Monastery.Reading.State;

public record BlockBody
{
    private int level = 0;
    public BlockBody Enter() => this with { level = level + 1 };
    public BlockBody Exit() { if (level == 1) Done = true; return this with { level = level - 1 }; }
    public bool InBody => level > 0;
    public bool Done { get; private set; } = false;
    private bool isNotFirstLine = false;
    public bool IsNotFirstLine()
    {
        var result = isNotFirstLine;
        isNotFirstLine = true;
        return result;
    }
}
