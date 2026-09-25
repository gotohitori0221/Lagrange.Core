using Lagrange.Proto.Generator.Utility.Extension;
using Lagrange.Proto.Serialization;
using Microsoft.CodeAnalysis;

namespace Lagrange.Proto.Generator.Utility;

public static class ProtoHelper
{
    public static byte[] EncodeVarInt(uint value)
    {
        Span<byte> result = stackalloc byte[5];
        int i = 0;
        while (value > 127)
        {
            result[i] = (byte)((value & 0x7F) | 0x80);
            value >>= 7;
            i++;
        }
        result[i] = ((byte)value);
        
        return result.Slice(0, i + 1).ToArray();
    }
    
    public static WireType GetWireType(ITypeSymbol symbol)
    {
        if (SymbolResolver.IsRepeatedType(symbol, out var type)) symbol = type;
        if (SymbolResolver.IsNullableType(symbol, out var elementType)) symbol = elementType;
        
        if (symbol.IsIntegerType() || symbol.TypeKind == TypeKind.Enum || symbol.SpecialType == SpecialType.System_Boolean) return WireType.VarInt;
        if (symbol.SpecialType == SpecialType.System_Single) return WireType.Fixed32;
        if (symbol.SpecialType == SpecialType.System_Double) return WireType.Fixed64;

        return WireType.LengthDelimited;
    }
}