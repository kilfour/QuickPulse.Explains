using QuickPulse.Arteries;
using QuickPulse.Explains.BasedOnNamespace;
using QuickPulse.Instruments;

namespace QuickPulse.Explains;

public static class Explain
{
    public static void This<T>(string filename)
    {
        var book = TheArchivist.Compose<T>();
        Signal.From(Rivers.Book)
            .SetArtery(WriteData.ToNewFile(filename))
            .Pulse(book);
    }

    public static void These<T>(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            ComputerSays.No($"Path: '{rootPath}' is not valid.");

        var book = TheArchivist.Compose<T>();

        var signal = Signal.From(Rivers.DocFile);
        foreach (var page in book.Pages)
        {
            var artery = WriteData.ToNewFile(Path.Combine(rootPath, page.Path));
            signal.SetArtery(artery).Pulse((page, book.Includes));
        }

        Signal.From(Rivers.ToC)
              .SetArtery(WriteData.ToNewFile(Path.Combine(rootPath, "ToC.md")))
              .Pulse(book.Pages.Select(a => new ToCEntry(a.DocFileExplained.HeaderText, a.Path)));
    }
}