using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

public static class CodeExampleExtractor
{
    public static ExtractedCode Extract(string filePath, int lineNumber)
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

        var declaration = FindDeclarationMarkedAtLine(root, lineNumber)
            ?? throw new InvalidOperationException(
                $"Could not find a declaration marked with CodeExampleAttribute on line {lineNumber} in '{filePath}'.");

        var stripped = (MemberDeclarationSyntax)new StripAllAttributesRewriter().Visit(declaration)!;

        var span = declaration.GetLocation().GetLineSpan();

        return new ExtractedCode(
            filePath,
            span.StartLinePosition.Line + 1,
            span.EndLinePosition.Line + 1,
            stripped.ToFullString().Trim());
    }

    private static MemberDeclarationSyntax? FindDeclarationMarkedAtLine(SyntaxNode root, int lineNumber)
    {
        foreach (var member in root.DescendantNodes().OfType<MemberDeclarationSyntax>())
        {
            var lists = GetAttributeLists(member);
            if (lists.Count == 0)
                continue;

            foreach (var list in lists)
            {
                if (!ContainsCodeExampleAttribute(list))
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

    private static bool ContainsCodeExampleAttribute(AttributeListSyntax list) =>
        list.Attributes.Any(a => IsCodeExampleName(a.Name.ToString()));

    private static bool IsCodeExampleName(string name) =>
        name == "CodeExample"
        || name == "CodeExampleAttribute"
        || name.EndsWith(".CodeExample", StringComparison.Ordinal)
        || name.EndsWith(".CodeExampleAttribute", StringComparison.Ordinal);

    private static bool ContainsLine(SyntaxNode node, int oneBasedLine)
    {
        var span = node.GetLocation().GetLineSpan();
        var start = span.StartLinePosition.Line + 1;
        var end = span.EndLinePosition.Line + 1;
        return oneBasedLine >= start && oneBasedLine <= end;
    }
}

public sealed record ExtractedCode(
    string File,
    int StartLine,
    int EndLine,
    string Code);

public sealed class StripAllAttributesRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node) =>
        base.VisitClassDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node) =>
        base.VisitStructDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitRecordDeclaration(RecordDeclarationSyntax node) =>
        base.VisitRecordDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) =>
        base.VisitInterfaceDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitEnumDeclaration(EnumDeclarationSyntax node) =>
        base.VisitEnumDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node) =>
        base.VisitMethodDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node) =>
        base.VisitConstructorDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node) =>
        base.VisitPropertyDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node) =>
        base.VisitFieldDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitEventFieldDeclaration(EventFieldDeclarationSyntax node) =>
        base.VisitEventFieldDeclaration(node.WithAttributeLists(default));

    public override SyntaxNode? VisitDelegateDeclaration(DelegateDeclarationSyntax node) =>
        base.VisitDelegateDeclaration(node.WithAttributeLists(default));
}