using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lagrange.Proto.Generator.Utility.Extension;

public static class RoslynExtension
{
    public static NameSyntax? GetNamespace(this MemberDeclarationSyntax context) => context.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name ?? context.Ancestors().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault()?.Name;

    public static bool ContainsAttribute(this MemberDeclarationSyntax context, string attributeName) => context.AttributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString() == attributeName);

    public static string GetTypeKindKeyword(this TypeDeclarationSyntax typeDeclaration)
    {
        switch (typeDeclaration.Kind())
        {
            case SyntaxKind.ClassDeclaration:
                return "class";
            case SyntaxKind.InterfaceDeclaration:
                return "interface";
            case SyntaxKind.StructDeclaration:
                return "struct";
            case SyntaxKind.RecordDeclaration:
                return "record";
            case SyntaxKind.RecordStructDeclaration:
                return "record struct";
            case SyntaxKind.EnumDeclaration:
                return "enum";
            case SyntaxKind.DelegateDeclaration:
                return "delegate";
            default:
                throw new NotSupportedException($"Unsupported type kind: {typeDeclaration.Kind()}");
        }
    }
}