using Lagrange.Proto.Serialization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Lagrange.Proto.Generator.Entity;

public class ProtoFieldInfo(ISymbol symbol, ITypeSymbol typeSymbol, WireType wireType, bool isSigned)
{
    public ISymbol Symbol { get; } = symbol;

    public ITypeSymbol TypeSymbol { get; } = typeSymbol;
    
    public WireType WireType { get; } = wireType;
    
    public bool IsSigned { get; } = isSigned;
    
    public List<ProtoTypeInfo> ExtraTypeInfo { get; } = [];
}
