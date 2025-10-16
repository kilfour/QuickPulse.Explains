using QuickPulse.Explains.Monastery;
using QuickPulse.Instruments;

namespace QuickPulse.Explains;

/// <summary>
/// Generates documentation artifacts from annotated tests.
/// </summary>
public static class Explain
{
    /// <summary>
    /// Generates documentation starting at the specified type and scans its namespace and descendant namespaces
    /// for other [DocFile] classes to include, writing the combined output to a single file.
    /// </summary>
    public static void This<T>(string filename)
    {
        var book = TheArchivist.Compose<T>();
        TheScribe.Print(filename, book);
    }

    /// <summary>
    /// Generates documentation only for the specified type, excluding related [DocFile] classes,
    /// and writes it to a single file.
    /// </summary>
    public static void OnlyThis<T>(string filename)
    {
        var book = TheArchivist.ComposeOnly<T>();
        TheScribe.Print(filename, book);
    }

    /// <summary>
    /// Generates documentation starting at the specified type and scans its namespace and descendant namespaces
    /// for other [DocFile] classes to include, publishing each as a separate file within the target folder.
    /// </summary>
    public static void These<T>(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) ComputerSays.No($"Path: '{path}' is not valid.");
        var book = TheArchivist.Compose<T>();
        TheScribe.Publish(path, book);
    }
}
