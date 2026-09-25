using Lagrange.Proto.Serialization;
using Microsoft.CodeAnalysis;

namespace Lagrange.Proto.Generator.Entity;

public class ProtoTypeInfo(ITypeSymbol typeSymbol, WireType wireType, bool isSigned)
{
    public ITypeSymbol TypeSymbol { get; } = typeSymbol;

    public WireType WireType { get; } = wireType;

    public bool IsSigned { get; } = isSigned;
}