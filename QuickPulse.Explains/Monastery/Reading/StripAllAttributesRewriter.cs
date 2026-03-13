using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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