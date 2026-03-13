namespace QuickPulse.Explains.Tests.CodeExampleTests.Methods.Models;

[DocFile]
[DocExample(typeof(MethodsModel), nameof(MethodOne))]
[DocExample(typeof(MethodsModel), nameof(MethodTwo))]
[DocExample(typeof(MethodsModel), nameof(MethodThree))]
[DocExample(typeof(MethodsModel), nameof(MethodFour))]
// [DocExample(typeof(MethodsModel), nameof(MethodFive))]
// [DocExample(typeof(MethodsModel), nameof(MethodSix))]
// [DocExample(typeof(MethodsModel), nameof(MethodSeven))]
// [DocExample(typeof(MethodsModel), nameof(MethodEight))]
// [DocExample(typeof(MethodsModel), nameof(MethodNine))]
// [DocExample(typeof(MethodsModel), nameof(MethodTen))]
public class MethodsModel
{
    [CodeExample]
    public int MethodOne() { return 42; }

    [CodeExample]
    public int MethodTwo()
    {
        return 42;
    }

    [CodeExample]
    public int MethodThree() => 42;

    [CodeExample]
    public int MethodFour()
        => 42;

    // [CodeExample]
    // public int MethodFive() { return 42; }

    // [CodeExample]
    // public int MethodSix() { return 42; }

    // [CodeExample]
    // public int MethodSeven() { return 42; }

    // [CodeExample]
    // public int MethodEight() { return 42; }

    // [CodeExample]
    // public int MethodNine() { return 42; }

    // [CodeExample]
    // public int MethodTen() { return 42; }
}