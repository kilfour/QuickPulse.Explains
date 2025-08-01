using QuickPulse.Explains.Deprecated;

namespace QuickPulse.Explains.Tests;

[Doc(Order = "0-1", Caption = "Attribute-Based Documentation", Content =
@"Apply the `[Doc]` attribute to your test classes and methods:

```csharp
[Doc(Order = ""1"", Caption = ""The Calculator"", Content = ""This class calculates totals."")]
public class CalculatorTests
{
    [Fact]
    [Doc(Order = ""1-1"", Caption = ""Empty Cart"", Content = ""An empty cart always totals to 0."")]
    public void EmptyCart_ReturnsZero() => ...
}
```")]
public class DocAttributeTests
{
    [Fact]
    [Doc(Order = "0-1-1", Caption = "", Content =
"The `Order` defines hierarchy (`1`, `1-1`, etc.), `Caption` becomes the header, and `Content` is the body text.")]
    public void Constructor_Sets_All_Properties()
    {
        var attr = new DocAttribute("1-1", "Caption", "Content");

        Assert.Equal("1-1", attr.Order);
        Assert.Equal("Caption", attr.Caption);
        Assert.Equal("Content", attr.Content);
    }

    [Fact]
    public void Constructor_Uses_Default_Empty_Strings()
    {
        var attr = new DocAttribute();

        Assert.Equal("", attr.Order);
        Assert.Equal("", attr.Caption);
        Assert.Equal("", attr.Content);
    }

    [Fact]
    public void Equals_Returns_True_For_Identical_Values()
    {
        var a = new DocAttribute("1", "Cap", "Text");
        var b = new DocAttribute("1", "Cap", "Text");

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_Returns_False_For_Different_Values()
    {
        var a = new DocAttribute("1", "Cap", "Text");
        var b = new DocAttribute("2", "Cap", "Text");

        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_Returns_False_For_Null()
    {
        var a = new DocAttribute("1", "Cap", "Text");

        Assert.False(a.Equals(null));
        Assert.False(a.Equals((object?)null));
    }
}