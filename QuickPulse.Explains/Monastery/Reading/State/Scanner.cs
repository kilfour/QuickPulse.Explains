namespace QuickPulse.Explains.Monastery.Reading.State;

public record Scanner(char LastChar = ' ', int Consumed = 0)
{
    public Scanner Consume(char ch) => new(ch, Consumed + 1);
    public static Scanner Reset() => new();
};
