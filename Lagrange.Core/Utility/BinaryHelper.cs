using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Lagrange.Core.Utility;

public static class BinaryHelper
{
    
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ReverseEndianness<T>(T value) where T : INumberBase<T> => value switch
    {
        sbyte sb => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(sb)),
        byte b => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(b)),
        char c => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(c)),
        short s => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(s)),
        ushort us => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(us)),
        int i => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(i)),
        uint u => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(u)),
        long l => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(l)),
        ulong ul => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(ul)),
        nint i => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(i)),
        nuint ul => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(ul)),
        Int128 i => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(i)),
        UInt128 ul => T.CreateTruncating(BinaryPrimitives.ReverseEndianness(ul)),
        _ => throw new NotSupportedException($"Type {typeof(T)} is not supported.")
    };
}