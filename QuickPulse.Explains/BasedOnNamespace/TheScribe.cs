using QuickPulse.Arteries;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheScribe
{
    public static void Print(string filename, Book book)
    {
        Signal.From(Scriptorium.Book)
            .SetArtery(WriteData.ToNewFile(filename))
            .Pulse(book);
    }

    public static void Publish(string path, Book book)
    {
        var signal = Signal.From(Scriptorium.SeperatePage);
        foreach (var page in book.Pages)
        {
            var artery = WriteData.ToNewFile(Path.Combine(path, page.Path));
            signal.SetArtery(artery).Pulse(new SeperatePage(page, book.Includes));
        }
        Signal.From(Scriptorium.Chronicles)
            .SetArtery(WriteData.ToNewFile(Path.Combine(path, "ToC.md")))
            .Pulse(book.Pages.Select(a => new Chronicle(a.Explanation.HeaderText, a.Path)));
    }
}