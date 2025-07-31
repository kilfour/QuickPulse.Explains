using System.Reflection;
using QuickPulse.Arteries;
using QuickPulse.Bolts;
using QuickPulse.Explains.BasedOnNamespace;
using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests;

public class Spike
{
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

    [Fact]
    public void GettingTheExample()
    {
        // var methods = typeof(Spike).GetDocAttributes<DocExampleAttribute>().ToList();
        // Signal.From<DocExampleAttribute>(a =>
        //         Pulse.Trace($"{a.File}:{a.Line}"))
        //     .SetArtery(WriteData.ToNewFile())
        //     .Pulse(methods);
    }

    [DocExample]
    public void Example()
    {
        string n = nameof(Example);
        Signal.ToFile<string>().Pulse(n);
    }
}

