using QuickPulse.Explains;

namespace _TestModel.AutoDocumentedThings;

[DocFile]
public class A_SubAutoDocumentedThingy
{
    [DocHeader("Auto DocTest Sub Method")]
    [DocContent("Some Sub explanation")]
    public void DoStuff() { }
}


