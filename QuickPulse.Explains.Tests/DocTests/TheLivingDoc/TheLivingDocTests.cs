namespace QuickPulse.Explains.Tests.DocTests.TheLivingDoc;

[DocFile]
public class TheLivingDocTests
{
    [DocContent(
@"**QuickPulse.Explains** supports embedding source code directly into generated documentation through
a set of Code... attributes.  
These attributes don't create documentation content themselves, instead,
they reference code from your project so it can be displayed alongside explanations, headers, and other doc elements.

`CodeExample` extracts a method or class for use as a worked example.

`CodeSnippet` extracts the body of a method, ignoring the signature.

`DocExample` uses the extractions from the above attributes and injects it into the generated markdown.

`CodeFile` pulls in an entire file.
The filename needs to be specified and the file to be included,
needs to be in the same folder as the class with the attribute.

All extracted code is formatted and syntax-highlighted automatically, preserving indentation and spacing.
This ensures your documentation always reflects the current, runnable source without manual copy-paste.

**Important:** The code extraction only works for methods with a *block body*,
it currently fails miserably when confronted with *expression bodied* methods.")]
    public void CodeInclusionSystem()
    {

    }
}