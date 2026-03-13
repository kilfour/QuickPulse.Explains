using QuickPulse.Explains.Exceptions.Abstractions;

namespace QuickPulse.Explains.Exceptions;

public class EmptyStringUsedInCodeReplaceAttributeException(string name)
    : QuickPulseExplainsException(GetMessage(name))
{
    private static string GetMessage(string name) =>
        FormatMessage($"Empty string used in CodeReplaceAttribute at '{name}'.");
}
