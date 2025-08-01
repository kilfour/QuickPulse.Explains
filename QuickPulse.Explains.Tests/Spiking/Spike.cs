using QuickPulse;
using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Explains.BasedOnNamespace;
using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests.Spiking;

[DocFile]
public class Spike
{

    public record Context(int Braces, int SpaceCount);
    [Fact]
    public void ParsingMethod()
    {
        var str =
@"public void ParsingMethod() 
{
    using(something)
    {
        var foo = int;
    }
}";
        var holden = TheString.Catcher();

        var findCodeFlow =
            from ch in Pulse.Start<char>()
            from _ in Pulse.ManipulateIf<Context>(ch == '}', a => a with { Braces = a.Braces - 1 })
            from __ in Pulse.TraceIf<Context>(a => a.Braces >= 0, () => ch)
            from ___ in Pulse.ManipulateIf<Context>(ch == '{', a => a with { Braces = a.Braces + 1 })
            select ch;

        var flow =
            from s in Pulse.Start<string>()
            let l = s.TakeWhile(char.IsWhiteSpace).Count()
            from _ in Pulse.Gather(new Context(-1, 0))
            select s;

        Signal.From<string>(
            a =>
                from cnt in Pulse.Gather(-1)
                from _ in Pulse.ToFlow(
                    c =>
                        from _ in Pulse.ManipulateIf<int>(c == '}', a => a - 1)
                        from __ in Pulse.TraceIf<int>(a => a >= 0, () => c)
                        from ___ in Pulse.ManipulateIf<int>(c == '{', a => a + 1)
                        select Unit.Instance, a)
                select Unit.Instance)
            .SetArtery(holden)
            .Pulse(str);
        var reader = LinesReader.FromText(holden.Whispers());
        Assert.Equal("", reader.NextLine());
        Assert.Equal("    using(something)", reader.NextLine());
        Assert.Equal("    {", reader.NextLine());
        Assert.Equal("        var foo = int;", reader.NextLine());
        Assert.Equal("    }", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    // [Fact]
    // [DocCodeExample(nameof(Example))]
    // public void GettingTheExample()
    // {
    //     Explain.This<Spike>("temp.md");
    // }

    // [DocExample]
    // private void Example()
    // {
    //     string n = nameof(Example);
    //     Signal.ToFile<string>().Pulse(n);
    // }
}

