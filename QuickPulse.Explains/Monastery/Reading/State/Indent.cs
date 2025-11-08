namespace QuickPulse.Explains.Monastery.Reading.State;

public record Indent
{
    public bool Emit(char ch) => decided ? IncrementAndCheck() : DecideOnIndentLevel(ch);
    public Indent Reset() { currentLevel = 0; return this; }
    public bool HasDecided => decided;
    private int level = 0;
    private int currentLevel = 0;
    private bool decided = false;
    private bool IncrementAndCheck() { currentLevel++; return currentLevel > level; }
    private bool DecideOnIndentLevel(char ch) => char.IsWhiteSpace(ch) ? StillThinking() : Decided();
    private bool StillThinking() { level++; return false; }
    private bool Decided() { currentLevel = level; decided = true; return true; }
}