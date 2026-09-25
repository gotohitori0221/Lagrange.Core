using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lagrange.Proto.Generator.Utility.Extension;

public static class TypeExtension
{
    private static readonly string[] SystemAssemblies = ["mscorlib", "System", "System.Core", "System.Private.CoreLib", "System.Runtime", "System.Collections"];
    
    public static bool IsUserDefinedType(this ITypeSymbol type)
    {
        if (type is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.IsGenericType)
            {
                return !namedTypeSymbol.ContainingAssembly.IsSystemAssembly() && !namedTypeSymbol.TypeArguments.All(IsUserDefinedType);
            }

            return !namedTypeSymbol.ContainingAssembly.IsSystemAssembly();
        }
        
        if (type is IArrayTypeSymbol arrayTypeSymbol)
        {
            return arrayTypeSymbol.ElementType.IsUserDefinedType();
        }

        return true;
    }
    
    private static bool IsSystemAssembly(this IAssemblySymbol assembly)
    {
        return SystemAssemblies.Contains(assembly.Name);
    }
    
    public static bool IsIntegerType(this ITypeSymbol symbol)
    {
        if (SymbolResolver.IsNullableType(symbol, out var type)) symbol = type;

        return (sbyte)symbol.SpecialType is >= (int)SpecialType.System_SByte and <= (int)SpecialType.System_UInt64;
    }
    
    public static bool IsNullable(this ITypeSymbol symbol)
    {
        return symbol is INamedTypeSymbol { IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T };
    }
    
    public static string GetFullName(this ITypeSymbol symbol)
    {
        if (symbol is INamedTypeSymbol namedTypeSymbol)
        {
            string namespaceName = namedTypeSymbol.ContainingNamespace.ToString();
            string typeName = namedTypeSymbol.Name;
            while (namedTypeSymbol.ContainingType != null)
            {
                namedTypeSymbol = namedTypeSymbol.ContainingType;
                typeName = $"{namedTypeSymbol.Name}.{typeName}";
            }
            
            if (namedTypeSymbol.IsGenericType)
            {
                string genericTypeArguments = string.Join(", ", namedTypeSymbol.TypeArguments.Select(t => t.GetFullName()));
                return $"global::{namespaceName}.{typeName}<{genericTypeArguments}>";
            }
            else
            {
                return $"global::{namespaceName}.{typeName}";
            }
        }
        
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
}