using QuickPulse.Arteries;
using QuickPulse.Explains.Monastery;
using QuickPulse.Explains.Tests._Tools;
using QuickPulse.Explains.Text;

namespace QuickPulse.Explains.Tests.Reference;

//[DocFile]
public class DocFileTests
{
    [Fact]
    [DocContent("When generating a filename from the class name `DocFile` ignores ending 'Tests'")]
    public void DocFile_Strips_Ending_Tests_From_Class_Name()
    {
        var type = DynamicTypeBuilder.Create("SampleClassTests")
            .WithClassAttribute<DocFileAttribute>()
            .Build();

        var stringSink = Arteries.Text.Capture();
        TheScribe.GetArtery = a => stringSink;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromText(stringSink.Content());
        Assert.Equal("# Sample Class", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocContent("(case sensitive).")]
    public void DocFile_Strips_Ending_Tests_From_Class_Name_case_sensitive()
    {
        var type = DynamicTypeBuilder.Create("SampleClasstests")
            .WithClassAttribute<DocFileAttribute>()
            .Build();

        var stringSink = Arteries.Text.Capture();
        TheScribe.GetArtery = a => stringSink;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromText(stringSink.Content());
        Assert.Equal("# Sample Classtests", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocContent("When generating a filename from the class name `DocFile` ignores ending 'Test'")]
    public void DocFile_Strips_Ending_Test_From_Class_Name()
    {
        var type = DynamicTypeBuilder.Create("SampleClassTest")
            .WithClassAttribute<DocFileAttribute>()
            .Build();

        var stringSink = Arteries.Text.Capture();
        TheScribe.GetArtery = a => stringSink;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromText(stringSink.Content());
        Assert.Equal("# Sample Class", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    [DocContent("(case sensitive).")]
    public void DocFile_Strips_Ending_Test_From_Class_Name_case_sensitive()
    {
        var type = DynamicTypeBuilder.Create("SampleClasstest")
            .WithClassAttribute<DocFileAttribute>()
            .Build();

        var stringSink = Arteries.Text.Capture();
        TheScribe.GetArtery = a => stringSink;

        ExplainThis.Invoke(type, "whatever");

        var reader = LinesReader.FromText(stringSink.Content());
        Assert.Equal("# Sample Classtest", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}