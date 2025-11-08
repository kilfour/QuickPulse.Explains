using QuickPulse.Arteries;

namespace QuickPulse.Explains.Monastery.CodeLocator;

public static class CodeReader
{
    private record Emitted(string Tracked = "")
    {
        public Emitted Track(char ch) => new(Tracked + ch);
        public static Emitted Reset() => new();
    };

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

    private record Scanner(char LastChar = ' ', int Consumed = 0)
    {
        public Scanner Consume(char ch) => new(ch, Consumed + 1);
        public static Scanner Reset() => new();
    };

    private enum BodyType { Unknown, Block, Expression }

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

    private record ExpressionBody(bool InBody)
    {
        private bool isNotFirstLine = false;
        public bool IsNotFirstLine()
        {
            var result = isNotFirstLine;
            isNotFirstLine = true;
            return result;
        }
    };

    private record Indent
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

    private static readonly Flow<char> Emit =
        Pulse.Start<char>(
            ch => Pulse.When<Indent>(a => a.Emit(ch),
            () => Pulse.Manipulate<Emitted>(a => a.Track(ch)).Dissipate()));

    private static readonly Flow<char> BlockBodiedMethodBodyChar =
        from ch in Pulse.Start<char>()
        from asExample in Pulse.Draw<bool>()
        from subExample in Pulse.When<BlockBody>(a => asExample && !a.Done, Pulse.ToFlow(Emit, ch))
        from exit in Pulse.ManipulateIf<BlockBody>(ch == '}', a => a.Exit())
        from sub in Pulse.When<BlockBody>(a => !asExample && a.InBody, Pulse.ToFlow(Emit, ch))
        from enter in Pulse.ManipulateIf<BlockBody>(ch == '{', a => a.Enter())
        from stop in Pulse.StopFlowingIf<BlockBody>(a => a.Done)
        select ch;

    private static readonly Flow<string> BlockBodiedMethodBody =
        from str in Pulse.Start<string>()
        from asExample in Pulse.Draw<bool>()
        from indent in Pulse.Manipulate<Indent>(a => a.Reset())
        from sub in Pulse.ToFlow(BlockBodiedMethodBodyChar, str)
        select str;

    private static readonly Flow<char> ExpressionBodiedMethodBodyChar =
        from ch in Pulse.Start<char>()
        from sub in Pulse.When<ExpressionBody>(a => a.InBody, Pulse.ToFlow(Emit, ch))
        from atEnd in Pulse.ManipulateIf<ExpressionBody>(ch == ';', a => new ExpressionBody(false))
        from stop in Pulse.StopFlowingIf<ExpressionBody>(a => !a.InBody)
        select ch;

    private static readonly Flow<string> ExpressionBodiedMethodBody =
        from str in Pulse.Start<string>()
        from reset in Pulse.Manipulate<Indent>(a => a.Reset())
        from sub in Pulse.ToFlow(ExpressionBodiedMethodBodyChar, str)
        select str;

    private static readonly Flow<char> DetermineBodyTypeChar =
        from ch in Pulse.Start<char>()
        from scanner in Pulse.Draw<Scanner>()
        from isBlock in Pulse.ManipulateIf<BodyType>(ch == '{', _ => BodyType.Block)
        from consumed in Pulse.When<BodyType>(
            a => a == BodyType.Unknown,
            () => Pulse.Manipulate<Scanner>(a => a.Consume(ch)).Dissipate())
        from isExpression in Pulse.ManipulateIf<BodyType>($"{scanner.LastChar}{ch}" == "=>", _ => BodyType.Expression)
        select ch;

    private static readonly Flow<string> DetermineBodyType =
        from str in Pulse.Start<string>()
        from scannerReset in Pulse.Manipulate<Scanner>(a => Scanner.Reset())
        from check in Pulse.ToFlow(DetermineBodyTypeChar, str)
        from scanner in Pulse.Draw<Scanner>()
        from cont in Pulse.When<BodyType>(
            a => a != BodyType.Unknown, () => Pulse.ToFlow(Line!, str[scanner.Consumed..].Trim()))
        select str;

    private static readonly Flow<char> GetSignatureChar =
        from ch in Pulse.Start<char>()
        from scanner in Pulse.Draw<Scanner>()
        from isBlock in Pulse.ManipulateIf<BodyType>(ch == '{', _ => BodyType.Block)
        from consumed in Pulse.When<BodyType>(
            a => a == BodyType.Unknown,
            () => Pulse.Manipulate<Scanner>(a => a.Consume(ch)).Dissipate())
        from isExpression in Pulse.ManipulateIf<BodyType>($"{scanner.LastChar}{ch}" == "=>", _ => BodyType.Expression)
        select ch;

    private static readonly Flow<string> GetSignature =
        from str in Pulse.Start<string>()
        from scanner in Pulse.Manipulate<Scanner>(a => Scanner.Reset())
        from indent in Pulse.Manipulate<Indent>(a => a.Reset())
        from check in Pulse.ToFlow(GetSignatureChar, str)
        from cont in Pulse.When<BodyType>(
            a => a != BodyType.Unknown, () => Pulse.ToFlow(Line!, str[scanner.Consumed..]))
        from trace in Pulse.When<BodyType>(
            a => a == BodyType.Unknown, () => Pulse.ToFlow(Emit, str))
        select str;

    private static readonly Flow<char> SkipAttributesChar =
        from ch in Pulse.Start<char>()
        from enter in Pulse.ManipulateIf<Attributes>(ch == '[', a => a.Enter())
        from done in Pulse.ManipulateIf<Attributes>(a => !char.IsWhiteSpace(ch) && !a.InAttribute, a => a with { Done = true })
        from remain in Pulse.ManipulateIf<Attributes>(a => !a.InAttribute, a => a.Remains(ch))
        from exit in Pulse.ManipulateIf<Attributes>(ch == ']', a => a.Exit())
        select ch;

    private static readonly Flow<string> SkipAttributes =
        from str in Pulse.Start<string>()
        from reset in Pulse.Manipulate<Attributes>(a => a.Reset())
        from sub in Pulse.ToFlow(SkipAttributesChar, str)
        from attrs in Pulse.Draw<Attributes>()
        from remain in Pulse.When<Attributes>(a => a.HasRemainder(), () => Pulse.ToFlow(Line!, attrs.Remainder))
        select str;

    private static readonly Flow<Flow> Flush =
        Pulse.TraceIf<Emitted>(a => !string.IsNullOrWhiteSpace(a.Tracked), a => a.Tracked);

    private static readonly Flow<string> Line =
        from str in Pulse.Start<string>()
        from bodyType in Pulse.Draw<BodyType>()
        from asExample in Pulse.Draw<bool>()
        from attrs in Pulse.Draw<Attributes>()
        from output in Flush
        from reset in Pulse.Manipulate<Emitted>(_ => Emitted.Reset())
        from _ in Pulse.FirstOf(
            (() => asExample && !attrs.Done, () => Pulse.ToFlow(SkipAttributes, str)),
            (() => bodyType == BodyType.Unknown && !asExample, () => Pulse.ToFlow(DetermineBodyType, str)),
            (() => bodyType == BodyType.Unknown && asExample, () => Pulse.ToFlow(GetSignature, str)),
            (() => bodyType == BodyType.Block, () => Pulse.ToFlow(BlockBodiedMethodBody, str)),
            (() => bodyType == BodyType.Expression, () => Pulse.ToFlow(ExpressionBodiedMethodBody, str)))
        select str;

    private static readonly Flow<Flow> PrimeState =
        from emmitted in Pulse.Prime(() => new Emitted())
        from scanner in Pulse.Prime(() => new Scanner())
        from attrs in Pulse.Prime(() => new Attributes())
        from bodyType in Pulse.Prime(() => BodyType.Unknown)
        from blockBody in Pulse.Prime(() => new BlockBody())
        from exprBody in Pulse.Prime(() => new ExpressionBody(true))
        from indent in Pulse.Prime(() => new Indent())
        select Flow.Continue;

    private static IEnumerable<string> GetResult(Flow<IEnumerable<string>> flow, IEnumerable<string> input)
        => Signal.From(flow)
            .SetArtery(Collect.ValuesOf<string>())
            .Pulse(input)
            .FlatLine(Flush)
            .GetArtery<Collector<string>>()
            .Values;

    private static readonly Flow<IEnumerable<string>> TheCode =
        Pulse.Start<IEnumerable<string>>(a => PrimeState.Then(Pulse.ToFlow(Line, a)));

    public static IEnumerable<string> AsSnippet(IEnumerable<string> input) =>
        GetResult(Pulse.Prime(() => false).Then(TheCode), input);

    public static IEnumerable<string> AsExample(IEnumerable<string> input) =>
        GetResult(Pulse.Prime(() => true).Then(TheCode), input);
}