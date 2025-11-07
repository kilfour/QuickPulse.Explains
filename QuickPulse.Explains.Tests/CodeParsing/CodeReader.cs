using System.Runtime.CompilerServices;
using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Explains.Tests.CodeParsing;


public record CurlyBraces
{
    private int level = 0;
    public CurlyBraces Enter() => this with { level = level + 1 };
    public CurlyBraces Exit() { if (level == 1) Done = true; return this with { level = level - 1 }; }
    public bool InBody => level > 0;
    public bool Done { get; private set; } = false;
}

public record Indent
{
    private int level = 0;
    private int currentLevel = 0;
    private bool decided = false;
    public bool Emit(char ch)
    {
        if (!decided)
        {
            if (char.IsWhiteSpace(ch))
            {
                level++;
                return false;
            }
            else
            {
                currentLevel = level;
                decided = true;
                return true;
            }
        }
        else
        {
            currentLevel++;
            return currentLevel > level;
        }
    }

    public Indent Reset() { currentLevel = 0; return this; }
}

public static class CodeReader
{
    private enum BodyType { Unknown, Block, Expression }

    private record ExpressionBody(bool InBody);

    private record Scanner(char LastChar = ' ', int Consumed = 0)
    {
        public Scanner Consume(char ch) => new(ch, Consumed + 1);
        public static Scanner Reset() => new();
    };

    private static readonly Flow<char> InBodyChar =
        Pulse.Start<char>(ch => Pulse.TraceIf<Indent>(a => a.Emit(ch), () => ch));

    private static readonly Flow<char> BlockBodiedMethodBodyChar =
        from ch in Pulse.Start<char>()
        from _2 in Pulse.ManipulateIf<CurlyBraces>(ch == '}', a => a.Exit())
        from _3 in Pulse.ToFlowIf<char, CurlyBraces>(a => a.InBody, InBodyChar, () => ch)
        from _4 in Pulse.ManipulateIf<CurlyBraces>(ch == '{', a => a.Enter())
        from stop in Pulse.StopFlowingIf<CurlyBraces>(a => a.Done)
        select ch;

    private static readonly Flow<string> BlockBodiedMethodBody =
        from str in Pulse.Start<string>()
        from primeBraces in Pulse.Prime(() => new CurlyBraces())
        from indent in Pulse.Prime(() => new Indent())
        let reset = indent.Reset()
        from sub in Pulse.ToFlow(BlockBodiedMethodBodyChar, str)
        from braces in Pulse.Draw<CurlyBraces>()
        from first in Pulse.Prime(() => true)
        from newline in Pulse.TraceIf(braces.InBody && !first, () => Environment.NewLine)
        from setFirst in Pulse.ManipulateIf<bool>(braces.InBody && first, _ => false)
        select str;

    private static readonly Flow<char> ExpressionBodiedMethodBodyChar =
        from ch in Pulse.Start<char>()
        from sub in Pulse.ToFlowIf<char, ExpressionBody>(a => a.InBody, InBodyChar, () => ch)
        from atEnd in Pulse.ManipulateIf<ExpressionBody>(ch == ';', a => new ExpressionBody(false))
        from stop in Pulse.StopFlowingIf<ExpressionBody>(a => !a.InBody)
        select ch;

    private static readonly Flow<string> ExpressionBodiedMethodBody =
        from str in Pulse.Start<string>()
        from indent in Pulse.Prime(() => new Indent())
        let reset = indent.Reset()
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

    private static readonly Flow<string> DetermineBodyTypeContinue =
        from str in Pulse.Start<string>()
        from scanner in Pulse.Draw<Scanner>()
        from cont in Pulse.ToFlow(Line!, str[scanner.Consumed..])
        select str;

    private static readonly Flow<string> DetermineBodyType =
        from str in Pulse.Start<string>()
        from scanner in Pulse.Manipulate<Scanner>(a => Scanner.Reset())
        from check in Pulse.ToFlow(DetermineBodyTypeChar, str)
        from cont in Pulse.When<BodyType>(
            a => a != BodyType.Unknown, () => Pulse.ToFlow(DetermineBodyTypeContinue, str))
        select str;

    private static readonly Flow<string> Line =
        from str in Pulse.Start<string>()
        from scanner in Pulse.Prime(() => new Scanner())
        from bodyType in Pulse.Prime(() => BodyType.Unknown)
        from exprBody in Pulse.Prime(() => new ExpressionBody(true))
        from _ in Pulse.FirstOf(
            (() => bodyType == BodyType.Unknown, () => Pulse.ToFlow(DetermineBodyType, str)),
            (() => bodyType == BodyType.Block, () => Pulse.ToFlow(BlockBodiedMethodBody, str)),
            (() => bodyType == BodyType.Expression, () => Pulse.ToFlow(ExpressionBodiedMethodBody, str)))
        select str;
    public static string Process(string[] input)
    {
        return
            Signal.From(Line)
                .SetArtery(Arteries.Text.Capture())
                .Pulse(input)
                .GetArtery<StringSink>()
                .Content();
    }
}