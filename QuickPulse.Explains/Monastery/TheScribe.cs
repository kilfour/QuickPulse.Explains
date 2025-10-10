using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickPulse.Explains.Monastery;

public static class TheScribe
{
    private static readonly AsyncLocal<Func<string, IArtery>?> _override = new();

    public static Func<string, IArtery> GetArtery
    {
        get => _override.Value ?? (a => TheLedger.Rewrites(a));
        set => _override.Value = value;
    }

    public static void Print(string filename, Book book)
    {
        var diagnostics = new Diagnostics();
        Signal.From(Scriptorium.Book)
            .SetArtery(GetArtery(filename))
            .Graft(diagnostics)
            .Pulse(book);
        CheckDiagnostics(diagnostics);
    }

    public static void Publish(string path, Book book)
    {
        var signal = Signal.From(Scriptorium.SinglePage);
        foreach (var page in book.Pages)
        {
            var artery = GetArtery(Path.Combine(path, page.Path));
            signal.SetArtery(artery).Pulse(new SinglePage(page, book.Inclusions, book.Examples));
        }
        var diagnostics = new Diagnostics();
        Signal.From(Scriptorium.TableOfContent)
            .SetArtery(GetArtery(Path.Combine(path, "ToC.md")))
            .Graft(diagnostics)
            .Pulse(book.Pages.Select(a => new Chronicle(a.Explanation.HeaderText, a.Path)));
        CheckDiagnostics(diagnostics);
    }

    private static void CheckDiagnostics(Diagnostics diagnostics)
    {
        if (diagnostics.TheExhibit.Count != 0)
            ComputerSays.No(
                "\r\n-----------------------------------------------------------------------\r\n" +
                string.Join(Environment.NewLine, diagnostics.TheExhibit));
    }
}
