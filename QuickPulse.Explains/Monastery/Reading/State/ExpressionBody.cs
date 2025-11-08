namespace QuickPulse.Explains.Monastery.Reading.State;

public record ExpressionBody(bool InBody)
{
    private bool isNotFirstLine = false;
    public bool IsNotFirstLine()
    {
        var result = isNotFirstLine;
        isNotFirstLine = true;
        return result;
    }
};
