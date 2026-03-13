using QuickPulse.Explains.Exceptions.Abstractions;

namespace QuickPulse.Explains.Exceptions;

public class DocRawFileNotFoundException(string filePath)
    : QuickPulseExplainsException(GetMessage(filePath))
{
    private static string GetMessage(string filePath) =>
        FormatMessage($"DocRawFile not found at '{filePath}'.");
}