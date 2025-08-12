
using System.Text.RegularExpressions;
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

    private static Flow<char> Enclosed(char enter, char exit, Flow<char> nextFlow) =>
            from c in Pulse.Start<char>()
            from _ in Pulse.Gather(new FlowContext())
            from __ in Pulse.ManipulateIf<FlowContext>(c == enter, a => a.Increment())
            from ___ in Pulse.ToFlowIf<char, FlowContext>(a => a.NeedsPrinting(c), nextFlow, () => c)
            from ____ in Pulse.ManipulateIf<FlowContext>(c == exit, a => a.Decrement())
            select c;

    public record FlowContext
    {
        public int Brackets { get; init; } = -1;

        public FlowContext Increment() => this with { Brackets = Brackets + 1 };

        public FlowContext Decrement() => this with { Brackets = Brackets - 1 };

        public bool NeedsPrinting(object c) => Brackets >= 0;
    }
}