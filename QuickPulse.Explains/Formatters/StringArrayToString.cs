
using System.Text.RegularExpressions;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace QuickPulse.Explains.Formatters;

public class StringArrayToString : ICodeFormatter
{
    public IEnumerable<string> Format(IEnumerable<string> lines)
    {
        var re = new Regex("\"((?:\\\\.|[^\"\\\\])*)\"", RegexOptions.Compiled);
        foreach (var line in lines)
            foreach (Match m in re.Matches(line))
                yield return m.Groups[1].Value;
    }

    // public IEnumerable<string> Format(IEnumerable<string> lines)
    // {
    //     var flow =
    //         from input in Pulse.Start<char>()
    //         from _ in Pulse.Trace(input)
    //         select input;
    //     return
    //         Signal.From<string>(a => Pulse.ToFlow(Enclosed(Between(flow)), a))
    //             .SetArtery(TheString.Catcher())
    //             .Pulse(lines)
    //             .GetArtery<Holden>()
    //             .Whispers()
    //             .Split(Environment.NewLine);
    // }

    private static Flow<char> Enclosed(Flow<char> nextFlow) =>
        from c in Pulse.Start<char>()
        from _ in Pulse.Gather(new BracketEnclosure())
        from ___ in Pulse.ToFlowIf<char, BracketEnclosure>(a => a.InEnclosure(c), nextFlow, () => c)
        select c;

    private static Flow<char> Between(Flow<char> nextFlow) =>
        from c in Pulse.Start<char>()
        from _ in Pulse.Gather(new QuoteEnclosure())
        from ___ in Pulse.ToFlowIf<char, QuoteEnclosure>(a => a.InEnclosure(c), nextFlow, () => c)
        select c;

    public abstract record Enclosure
    {
        protected abstract char Enter { get; }
        protected abstract char Exit { get; }
        public int Level { get; private set; } = -1;
        public bool InEnclosure(char ch)
        {
            if (Enter == Exit)
                return InSameEncloser(ch);
            if (ch == Exit) Level--;
            var result = Level >= 0;
            if (ch == Enter) Level++;
            return result;
        }

        private bool InSameEncloser(char ch)
        {
            if (ch == Enter)
            {
                if (Level == 0)
                {
                    Level--;
                }
                else
                    Level++;
            }
            return Level >= 0 && ch != Enter;
        }
    }

    public record BracketEnclosure : Enclosure
    {
        protected override char Enter => '[';
        protected override char Exit => ']';
    }

    public record QuoteEnclosure : Enclosure
    {
        protected override char Enter => '"';
        protected override char Exit => '"';
    }
}

