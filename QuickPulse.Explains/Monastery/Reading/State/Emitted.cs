namespace QuickPulse.Explains.Monastery.Reading.State;

public record Emitted(string Tracked = "")
{
    public Emitted Track(char ch) => new(Tracked + ch);
    public static Emitted Reset() => new();
};
