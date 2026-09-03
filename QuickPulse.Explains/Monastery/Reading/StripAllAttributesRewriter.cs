using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public sealed class StripAllAttributesRewriter : CSharpSyntaxRewriter
{
    private static readonly HashSet<string> DocumentationAttributes =
    [
        "CodeExample",
        "CodeSnippet",
        "CodeReplace",
        "CodeRemove",
        "CodeFormat",
        "DocBarChart",
        "DocCode",
        "DocCodeFile",
        "DocColumn",
        "DocContent",
        "DocExample",
        "DocFile",
        "DocFileHeader",
        "DocHeader",
        "DocInclude",
        "DocLink",
        "DocRawFile",
        "DocTable"
    ];

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node) =>
        base.VisitClassDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitStructDeclaration(StructDeclarationSyntax node) =>
        base.VisitStructDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitRecordDeclaration(RecordDeclarationSyntax node) =>
        base.VisitRecordDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node) =>
        base.VisitInterfaceDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitEnumDeclaration(EnumDeclarationSyntax node) =>
        base.VisitEnumDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node) =>
        base.VisitMethodDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node) =>
        base.VisitConstructorDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node) =>
        base.VisitPropertyDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node) =>
        base.VisitFieldDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitEventFieldDeclaration(EventFieldDeclarationSyntax node) =>
        base.VisitEventFieldDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    public override SyntaxNode? VisitDelegateDeclaration(DelegateDeclarationSyntax node) =>
        base.VisitDelegateDeclaration(node.WithAttributeLists(WithoutDocumentationAttributes(node.AttributeLists)));

    private static SyntaxList<AttributeListSyntax> WithoutDocumentationAttributes(
        SyntaxList<AttributeListSyntax> attributeLists)
    {
        var result = new List<AttributeListSyntax>();
        foreach (var attributeList in attributeLists)
        {
            var attributes = attributeList.Attributes
                .Where(attribute => !IsDocumentationAttribute(attribute))
                .ToArray();
            if (attributes.Length > 0)
                result.Add(attributeList.WithAttributes(SyntaxFactory.SeparatedList(attributes)));
        }

        return SyntaxFactory.List(result);
    }

    private static bool IsDocumentationAttribute(AttributeSyntax attribute)
    {
        var name = attribute.Name switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.ValueText,
            AliasQualifiedNameSyntax aliasQualified => aliasQualified.Name.Identifier.ValueText,
            _ => attribute.Name.ToString().Split(['.', ':']).Last()
        };
        if (name.EndsWith("Attribute", StringComparison.Ordinal))
            name = name[..^"Attribute".Length];
        return DocumentationAttributes.Contains(name);
    }
}
