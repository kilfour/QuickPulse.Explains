using QuickPulse.Arteries;

namespace QuickPulse.Explains.BasedOnNamespace;

public static class TheScribe
{
    private static readonly AsyncLocal<Func<string, IArtery>?> _override = new();

    public static Func<string, IArtery> GetArtery
    {
        get => _override.Value ?? WriteData.ToNewFile;
        set => _override.Value = value;
    }

    public static void Print(string filename, Book book)
    {
        Signal.From(Scriptorium.Book)
            .SetArtery(GetArtery(filename))
            .Pulse(book);
    }

    public static void Publish(string path, Book book)
    {
        var signal = Signal.From(Scriptorium.SeperatePage);
        foreach (var page in book.Pages)
        {
            var artery = GetArtery(Path.Combine(path, page.Path));
            signal.SetArtery(artery).Pulse(new SeperatePage(page, book.Includes, book.Examples));
        }
        Signal.From(Scriptorium.Chronicles)
            .SetArtery(GetArtery(Path.Combine(path, "ToC.md")))
            .Pulse(book.Pages.Select(a => new Chronicle(a.Explanation.HeaderText, a.Path)));
    }
}