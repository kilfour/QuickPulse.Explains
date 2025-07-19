using QuickPulse.Explains;

namespace DoNotPutThisInReadme;

[Doc("1", "Test Class", "This is a class-level doc.")]
public class DocumentedThing
{
    [Doc("1-1", "Test Method", "This is a method-level doc.")]
    public void DoStuff() { }

    // -- use the flags below if we _do_ need private method decorating
    // BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
    private void DoesNotShowUp() { }
}
