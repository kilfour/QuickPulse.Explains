using System.Text;
using QuickPulse.Explains.Monastery.Fragments;
using QuickPulse.Explains.Monastery.Writings;

namespace QuickPulse.Explains.Monastery;

public static class ScriptoriumImperative
{
    private const char Separator = '/';

    public static (string markdown, List<string> diagnostics) RenderBook(Book book)
    {
        var md = new StringBuilder();
        var diags = new List<string>();
        foreach (var page in book.Pages)
        {
            var level = SafeSplit(page.Path, Separator).Length;
            RenderExplanation(book, page.Explanation, md, diags, level);
        }
        return (md.ToString(), diags);
    }

    private static void RenderExplanation(
        Reference reference,
        Explanation explanation,
        StringBuilder md,
        List<string> diags,
        int level)
    {
        var headingMarker = new string('#', Math.Max(1, level));
        md.AppendLine($"{headingMarker} {explanation.HeaderText}");
        var innerLevel = level + 1;
        foreach (var fragment in explanation.Fragments)
        {
            RenderFragment(reference, fragment, md, diags, innerLevel);
        }
    }

    private static void RenderFragment(
        Reference reference,
        Fragment fragment,
        StringBuilder md,
        List<string> diags,
        int level)
    {
        switch (fragment)
        {
            case HeaderFragment h:
                {
                    var headingMarker = new string('#', Math.Max(1, level + h.Level));
                    md.AppendLine($"{headingMarker} {h.Header}");
                    break;
                }

            case ContentFragment c:
                {
                    md.AppendLine($"{c.Content}  ");
                    break;
                }

            case CodeFragment code:
                {
                    md.AppendLine($"```{code.Language}");
                    md.AppendLine(code.Code?.Trim() ?? string.Empty);
                    md.AppendLine("```");
                    break;
                }

            case CodeExampleFragment cx:
                {
                    md.AppendLine($"```{cx.Language}");
                    var example = reference.Examples.SingleOrDefault(e => e.Name == cx.Name);
                    if (example is not null)
                    {
                        md.AppendLine(example.Code);
                    }
                    else
                    {
                        diags.Add(GetErrorMessageFromCodeExampleFragmentName(cx.Name));
                    }
                    md.AppendLine("```");
                    break;
                }

            case InclusionFragment inc:
                {
                    var inclusion = reference.Inclusions.Single(a => a.Type == inc.Included);
                    RenderExplanation(reference, inclusion.Explanation, md, diags, level);
                    break;
                }

            case LinkFragment link:
                {
                    md.AppendLine(""); 
                    md.AppendLine($"[{link.Name}]: {link.Location}");
                    break;
                }

            default:
                break;
        }
    }

    private static string GetErrorMessageFromCodeExampleFragmentName(string name)
    {
        var result = "Code Example not found.\r\n" +
                     "Looking for: `" + name + "`.\r\n";
        if (name.Contains('`'))
        {
            result +=
                " => Are you referencing a generic type?\r\n" +
                "If so you need to supply the open generic type (f.i. `typeof(MyType<>)` instead of `typeof(MyType<string>)`).\r\n";
        }
        result += "-----------------------------------------------------------------------";
        return result;
    }

    private static string[] SafeSplit(string s, char sep)
    {
        if (string.IsNullOrEmpty(s)) return Array.Empty<string>();
        return s.Split(sep, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}

