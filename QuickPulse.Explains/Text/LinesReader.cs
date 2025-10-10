using System.Diagnostics;
using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Explains.Text;

public class LinesReader
{
    private readonly string[] lines;
    private int currentIndex = -1;

    private LinesReader(string[] lines)
    {
        this.lines = lines;
        if (lines.Length > 0)
            currentIndex = 0;
    }
    public static LinesReader FromText(string text) =>
        new(text.Split(Environment.NewLine));

    public static LinesReader FromStringList(string[] lines) =>
        new(lines);

    [StackTraceHidden]
    public string NextLine()
    {
        if (currentIndex == -1)
            ComputerSays.No("No text was provided to the reader.");

        if (currentIndex >= lines.Length)
            ComputerSays.No("Attempted to read past the end of content.");

        return lines[currentIndex++];
    }

    [StackTraceHidden]
    public string[] NextLines(int howMany)
    {
        List<string> result = [];
        for (int i = 0; i < howMany; i++)
        {
            result.Add(NextLine());
        }
        return [.. result];
    }

    [StackTraceHidden]
    public void Skip() => currentIndex++;
    [StackTraceHidden]
    public void Skip(int linesToSkip) => currentIndex += linesToSkip;
    public string SkipToLineContaining(string fragment)
    {
        while (true)
        {
            var line = NextLine();
            if (line.Contains(fragment, StringComparison.OrdinalIgnoreCase))
                return line;
        }
    }

    [StackTraceHidden]
    public bool EndOfContent()
    {
        if (currentIndex < lines.Length)
            ComputerSays.No($"Not end of content: '{NextLine()}'.");
        return true;
    }

    public LinesReader AsAssertsToLogFile()
    {
        var indent = new string(' ', 8);
        string assert(string str) => $"{indent}Assert.Equal(\"{str.Replace("\"", "\\\"")}\", reader.NextLine());";
        var endOfContent = $"{indent}Assert.True(reader.EndOfContent());";
        Signal.From<string[]>(list =>
                Pulse.ToFlow(element => Pulse.Trace(assert(element)), list)
                .Then(Pulse.Trace(endOfContent)))
            .SetArtery(TheLedger.Records())
            .Pulse(lines);
        return this;
    }
}
