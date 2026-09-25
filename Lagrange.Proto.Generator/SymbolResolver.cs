using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Lagrange.Proto.Generator;

public class SymbolResolver
{
    private const string NodesTypeRef = "Lagrange.Proto.Nodes";

    private static readonly string[] Nodes = ["ProtoNode", "ProtoValue", "ProtoArray", "ProtoObject"];

    private static readonly string[] DynamicNodes = ["ProtoNode", "ProtoValue", "ProtoArray"];
    
    public static bool IsNodesType(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.ContainingNamespace.ToString() == NodesTypeRef && (Nodes.Contains(namedTypeSymbol.Name) || namedTypeSymbol is { IsGenericType: true, Name: "ProtoValue" });
        }

        return false;
    }
    
    public static bool IsDynamicNodesType(ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.ContainingNamespace.ToString() == NodesTypeRef && (DynamicNodes.Contains(namedTypeSymbol.Name) || namedTypeSymbol is { IsGenericType: true, Name: "ProtoValue" });
        }

        return false;
    }
    
    public static bool IsRepeatedType(ITypeSymbol type, [NotNullWhen(true)] out ITypeSymbol? elementType)
    {
        elementType = null;

        if (type is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol is { IsGenericType: true, ConstructedFrom.Name: "List" } &&
                namedTypeSymbol.ContainingNamespace.ToString() == "System.Collections.Generic")
            {
                elementType = namedTypeSymbol.TypeArguments[0];
                return true;
            }
        }
        else if (type is IArrayTypeSymbol arrayType && arrayType.ElementType.SpecialType != SpecialType.System_Byte)
        {
            elementType = arrayType.ElementType;
            return true;
        }

        return false;
    }
    
    public static bool IsMapType(
        ITypeSymbol type, 
        [NotNullWhen(true)] out ITypeSymbol? keyType, 
        [NotNullWhen(true)] out ITypeSymbol? valueType)
    {
        keyType = null;
        valueType = null;

        if (type is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol is { IsGenericType: true, ConstructedFrom.Name: "Dictionary" } &&
                namedTypeSymbol.ContainingNamespace.ToString() == "System.Collections.Generic")
            {
                keyType = namedTypeSymbol.TypeArguments[0];
                valueType = namedTypeSymbol.TypeArguments[1];
                return true;
            }
        }

        return false;
    }
    
    public static bool IsNullableType(ITypeSymbol type, [NotNullWhen(true)] out ITypeSymbol? elementType)
    {
        elementType = null;

        if (type is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol is { IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T })
            {
                elementType = namedTypeSymbol.TypeArguments[0];
                return true;
            }
        }

        return false;
    }

    public static bool IsProtoPackable(ITypeSymbol type) => type.GetAttributes().Any(x => x.AttributeClass?.ToDisplayString() == "Lagrange.Proto.ProtoPackableAttribute");

    public static ITypeSymbol GetGenericTypeNonNull(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            var genericType = namedTypeSymbol.ConstructedFrom;
            if (genericType.IsGenericType) return namedTypeSymbol.TypeArguments[0];
        }
        
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            var elementType = arrayTypeSymbol.ElementType;
            if (elementType is INamedTypeSymbol { ConstructedFrom.IsGenericType: true } namedElementType) return namedElementType.TypeArguments[0];
            return arrayTypeSymbol.ElementType;
        }
        
        return typeSymbol;
    }
    
    private static ITypeSymbol? GetGenericType(ITypeSymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            var genericType = namedTypeSymbol.ConstructedFrom;
            if (genericType.IsGenericType) return namedTypeSymbol.TypeArguments[0];
        }

        return null;
    }
}