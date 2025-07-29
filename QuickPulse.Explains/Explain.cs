using QuickPulse.Explains.BasedOnNamespace;
using QuickPulse.Instruments;

namespace QuickPulse.Explains;

public static class Explain
{
    public static void This<T>(string filename)
    {
        var book = TheArchivist.Compose<T>();
        TheScribe.Print(filename, book);
    }

    public static void These<T>(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) ComputerSays.No($"Path: '{path}' is not valid.");
        var book = TheArchivist.Compose<T>();
        TheScribe.Publish(path, book);
    }
}