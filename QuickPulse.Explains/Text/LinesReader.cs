using System.Diagnostics;
using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Explains.Text;

/// <summary>
/// Utility for reading multi-line text sequentially in tests,
/// providing simple navigation and assertion-friendly helpers.
/// </summary>
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

    /// <summary>
    /// Creates a reader from a text block, normalizing all line endings.
    /// </summary>
    public static LinesReader FromText(string text) =>
        new(Normalize(text).Split('\n'));

    [StackTraceHidden]
    private static string Normalize(string s)
        => s.Replace("\r\n", "\n").Replace("\r", "\n");

    /// <summary>
    /// Creates a reader from a pre-split list of lines.
    /// </summary>
    public static LinesReader FromStringList(string[] lines) =>
        new(lines);

    /// <summary>
    /// Reads the next line and advances the cursor.
    /// Throws if no text or past the end of content.
    /// </summary>
    [StackTraceHidden]
    public string NextLine()
    {
        if (currentIndex == -1)
            ComputerSays.No("No text was provided to the reader.");

        if (currentIndex >= lines.Length)
            ComputerSays.No($"Attempted to read past the end of content.{Environment.NewLine} Last line: {lines[lines.Length - 1]}.");

        return lines[currentIndex++];
    }

    /// <summary>
    /// Reads and returns the specified number of lines in sequence.
    /// </summary>
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

    /// <summary>
    /// Skips the next line without reading it.
    /// </summary>
    [StackTraceHidden]
    public void Skip() => currentIndex++;

    /// <summary>
    /// Skips the specified number of lines without reading them.
    /// </summary>
    [StackTraceHidden]
    public void Skip(int linesToSkip) => currentIndex += linesToSkip;

    /// <summary>
    /// Advances until a line containing the given fragment is found and returns it.
    /// </summary>
    public string SkipToLineContaining(string fragment)
    {
        while (true)
        {
            var line = NextLine();
            if (line.Contains(fragment, StringComparison.OrdinalIgnoreCase))
                return line;
        }
    }

    /// <summary>
    /// Asserts that all lines have been consumed, fails if not.
    /// </summary>
    [StackTraceHidden]
    public bool EndOfContent()
    {
        if (currentIndex < lines.Length)
            ComputerSays.No($"Not end of content: '{NextLine()}'.");
        return true;
    }

    /// <summary>
    /// Converts the current content into xUnit-style Assert lines
    /// and traces them to a test log for debugging.
    /// </summary>
    public LinesReader AsAssertsToLogFile()
    {
        var indent = new string(' ', 8);
        string assert(string str) => $"{indent}Assert.Equal(\"{str.Replace("\"", "\\\"")}\", reader.NextLine());";
        var endOfContent = $"{indent}Assert.True(reader.EndOfContent());";
        Signal.From<string[]>(list =>
                Pulse.ToFlow(element => Pulse.Trace(assert(element)), list)
                .Then(Pulse.Trace(endOfContent)))
            .SetArtery(FileLog.Append())
            .Pulse(lines);
        return this;
    }
}

