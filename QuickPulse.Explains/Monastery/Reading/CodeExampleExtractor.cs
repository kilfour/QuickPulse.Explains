using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace QuickPulse.Explains.Monastery.Reading;

public static class CodeExampleExtractor
{
    public static string Extract(string filePath, int lineNumber, bool asSnippet)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new InvalidOperationException("Code example file path is missing.");

        if (lineNumber == 0)
            throw new InvalidOperationException("Code example line number is missing.");

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Could not find source file.", filePath);

        var source = File.ReadAllText(filePath);
        var text = SourceText.From(source);
        var tree = CSharpSyntaxTree.ParseText(source, path: filePath);
        var root = tree.GetRoot();

        if (lineNumber < 1 || lineNumber > text.Lines.Count)
            throw new InvalidOperationException(
                $"Line {lineNumber} is outside the file '{filePath}'.");

        var declaration = FindDeclarationMarkedAtLine(root, lineNumber, asSnippet)
            ?? throw new InvalidOperationException(
                $"Could not find a declaration marked with CodeExampleAttribute on line {lineNumber} in '{filePath}'.");

        var stripped = (MemberDeclarationSyntax)new StripAllAttributesRewriter().Visit(declaration)!;

        if (asSnippet)
            return ExtractBody(stripped).Trim();

        return stripped.ToFullString().Trim();
    }

    private static MemberDeclarationSyntax? FindDeclarationMarkedAtLine(SyntaxNode root, int lineNumber, bool asSnippet)
    {
        foreach (var member in root.DescendantNodes().OfType<MemberDeclarationSyntax>())
        {
            var lists = GetAttributeLists(member);
            if (lists.Count == 0)
                continue;
            foreach (var list in lists)
            {
                if (!ContainsCodeAttribute(list, asSnippet))
                    continue;
                if (ContainsLine(list, lineNumber))
                    return member;
            }
        }
        return null;
    }

    private static SyntaxList<AttributeListSyntax> GetAttributeLists(MemberDeclarationSyntax member) =>
        member switch
        {
            ClassDeclarationSyntax x => x.AttributeLists,
            StructDeclarationSyntax x => x.AttributeLists,
            RecordDeclarationSyntax x => x.AttributeLists,
            InterfaceDeclarationSyntax x => x.AttributeLists,
            EnumDeclarationSyntax x => x.AttributeLists,
            MethodDeclarationSyntax x => x.AttributeLists,
            ConstructorDeclarationSyntax x => x.AttributeLists,
            PropertyDeclarationSyntax x => x.AttributeLists,
            FieldDeclarationSyntax x => x.AttributeLists,
            EventFieldDeclarationSyntax x => x.AttributeLists,
            DelegateDeclarationSyntax x => x.AttributeLists,
            _ => default
        };

    private static bool ContainsCodeAttribute(AttributeListSyntax list, bool asSnippet) =>
        list.Attributes.Any(a => IsCodeExampleName(a.Name.ToString(), asSnippet ? "CodeSnippet" : "CodeExample"));

    private static bool IsCodeExampleName(string name, string attributeString) =>
        name == attributeString
        || name == $"{attributeString}Attribute"
        || name.EndsWith($".{attributeString}", StringComparison.Ordinal)
        || name.EndsWith($".{attributeString}Attribute", StringComparison.Ordinal);

    private static bool IsCodeSnippetName(string name) =>
        name == "CodeSnippet"
        || name == "CodeSnippetAttribute"
        || name.EndsWith(".CodeSnippet", StringComparison.Ordinal)
        || name.EndsWith(".CodeSnippetAttribute", StringComparison.Ordinal);

    private static bool ContainsLine(SyntaxNode node, int oneBasedLine)
    {
        var span = node.GetLocation().GetLineSpan();
        var start = span.StartLinePosition.Line + 1;
        var end = span.EndLinePosition.Line + 1;
        return oneBasedLine >= start && oneBasedLine <= end;
    }

    private static string ExtractBlockContents(BlockSyntax block)
    {
        var text = block.ToFullString();

        var open = text.IndexOf('{');
        var close = text.LastIndexOf('}');

        if (open < 0 || close <= open)
            return string.Empty;

        return text.Substring(open + 1, close - open - 1);
    }

    private static string ExtractBody(MemberDeclarationSyntax declaration) =>
        declaration switch
        {
            MethodDeclarationSyntax m when m.Body is not null
                => ExtractBlockContents(m.Body).Trim(),

            MethodDeclarationSyntax m when m.ExpressionBody is not null
                => m.ExpressionBody.Expression.ToFullString().Trim() + ";",

            ConstructorDeclarationSyntax c when c.Body is not null
                => ExtractBlockContents(c.Body).Trim(),

            ConstructorDeclarationSyntax c when c.ExpressionBody is not null
                => c.ExpressionBody.Expression.ToFullString().Trim() + ";",

            _ => throw new InvalidOperationException("Body extraction is only supported for methods and constructors.")
        };
}