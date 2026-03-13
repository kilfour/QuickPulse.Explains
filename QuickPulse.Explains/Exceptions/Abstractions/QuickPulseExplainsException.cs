namespace QuickPulse.Explains.Exceptions.Abstractions;

public abstract class QuickPulseExplainsException : Exception
{
    public QuickPulseExplainsException(string message) : base(message) { }
    public QuickPulseExplainsException(string message, Exception exception) : base(message, exception) { }

    protected static string FormatMessage(string message)
        =>
@$"
-------------------------
-- QuickPulse.Explains --
-------------------------
{message}
-------------------------";
}