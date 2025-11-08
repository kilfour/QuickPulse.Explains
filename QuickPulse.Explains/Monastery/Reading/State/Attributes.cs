namespace QuickPulse.Explains.Monastery.Reading.State;

public record Attributes
{
    private int level = 0;
    public Attributes Enter() => this with { level = level + 1 };
    public Attributes Exit() { return this with { level = level - 1 }; }
    public bool InAttribute => level > 0;
    public string Remainder { get; private set; } = string.Empty;
    public Attributes Remains(char ch) { Remainder += ch; return this; }
    public bool Done { get; set; }
    public bool HasRemainder() => Done && !string.IsNullOrWhiteSpace(Remainder);
    public Attributes Reset() { Remainder = string.Empty; return this; }
}
