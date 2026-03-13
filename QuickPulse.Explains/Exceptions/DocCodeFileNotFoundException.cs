using QuickPulse.Explains.Exceptions.Abstractions;

namespace QuickPulse.Explains.Exceptions;

public class DocCodeFileNotFoundException(string filePath)
    : QuickPulseExplainsException(GetMessage(filePath))
{
    private static string GetMessage(string filePath) =>
        FormatMessage($"DocCodeFile not found at '{filePath}'.");
}
