using QuickPulse.Explains.BasedOnNamespace;

namespace QuickPulse.Explains.Tests.NamespaceBased;

public class AttributeTests
{
    [Fact]
    public void Test()
    {
        var result = ThePathfinder.NamespaceRelativeFilename(typeof(ReadMe), typeof(ReadMe));
        Assert.Equal("ReadMe.md", result);

        result = ThePathfinder.NamespaceRelativeFilename(typeof(ReadMe), typeof(AttributeTests));
        Assert.Equal("NamespaceBased\\AttributeTests.md", result);
    }
}