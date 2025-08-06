using System.Reflection;
using System.Reflection.Emit;
using QuickPulse.Arteries;
using QuickPulse.Explains.BasedOnNamespace;
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
        var asmName = new AssemblyName("DocIncludeTestingAssembly");
        var asm = AssemblyBuilder.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.Run);
        var module = asm.DefineDynamicModule("Main");
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            if (args.Name.StartsWith(asm.FullName!))
                return asm;
            return null;
        };

        var includedType = DynamicTypeBuilder.Create("SomeOtherClass", module)
            .WithVoidMethod<DocHeaderAttribute>("MyMethod", "Header From SomeOtherClass Method", 0)
            .Build();

        var type = DynamicTypeBuilder.Create("ASimpleFile", module)
            .WithClassAttribute<DocFileAttribute>()
            .WithClassAttribute<DocIncludeAttribute>(includedType)
            .Build();

        var collector = new TheCollector<string>();
        TheScribe.GetArtery = a => collector;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromStringList([.. collector.TheExhibit]);
        Assert.Equal("# A Simple File", reader.NextLine());
        Assert.Equal("## Some Other Class", reader.NextLine());
        Assert.Equal("### Header From SomeOtherClass Method", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}