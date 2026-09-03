using QuickPulse.Arteries;
using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Tests._Tools;
using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests.Reference;

//[DocFile]
public class DocIncludeTests
{
    [Fact]
    [DocContent("Also works on Class definition.")]
    public void DocInclude_Works_on_class()
    {
        using var module = DynamicModuleBuilder.Create();

        var includedType = DynamicTypeBuilder.Create("SomeOtherClass", module)
            .WithVoidMethod<DocHeaderAttribute>("MyMethod", "Header From SomeOtherClass Method", 0)
            .Build();

        var type = DynamicTypeBuilder.Create("ASimpleFile", module)
            .WithClassAttribute<DocFileAttribute>()
            .WithClassAttribute<DocIncludeAttribute>(includedType, false)
            .Build();

        var collector = Collect.ValuesOf<string>();
        var previousArtery = TheScribe.GetArtery;
        try
        {
            TheScribe.GetArtery = _ => collector;
            ExplainThis.Invoke(type, "whatever");
        }
        finally
        {
            TheScribe.GetArtery = previousArtery;
        }

        var reader = LinesReader.FromStringList([.. collector.Values]);
        Assert.Equal("# A Simple File", reader.NextLine());
        Assert.Equal("## Some Other Class", reader.NextLine());
        Assert.Equal("### Header From SomeOtherClass Method", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void DocInclude_NoHeader()
    {
        using var module = DynamicModuleBuilder.Create();

        var includedType = DynamicTypeBuilder.Create("SomeOtherClass", module)
            .WithVoidMethod<DocHeaderAttribute>("MyMethod", "Header From SomeOtherClass Method", 0)
            .Build();

        var type = DynamicTypeBuilder.Create("ASimpleFile", module)
            .WithClassAttribute<DocFileAttribute>()
            .WithClassAttribute<DocIncludeAttribute>(includedType, true)
            .Build();

        var collector = Collect.ValuesOf<string>();
        var previousArtery = TheScribe.GetArtery;
        try
        {
            TheScribe.GetArtery = _ => collector;
            ExplainThis.Invoke(type, "whatever");
        }
        finally
        {
            TheScribe.GetArtery = previousArtery;
        }

        var reader = LinesReader.FromStringList([.. collector.Values]);
        Assert.Equal("# A Simple File", reader.NextLine());
        Assert.Equal("### Header From SomeOtherClass Method", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}
