using QuickPulse.Explains.Exceptions.Abstractions;

namespace QuickPulse.Explains.Exceptions;


public class CodeExampleNotFoundException(string name)
    : QuickPulseExplainsException(GetMessage(name))
{
    private static string GetMessage(string name)
    {
        var result = BasicMessage(name);
        if (name.Contains('`'))
            result += GenericsHint;
        return FormatMessage(result);
    }

    private static string BasicMessage(string name) =>
@$"Code Example not found.
Looking for: '{name}'.";


    private const string GenericsHint =
@$"
 => Are you referencing a generic type?
If so you need to supply the open generic type (f.i. 'typeof(MyType<>)' instead of 'typeof(MyType<string>)')";
}
