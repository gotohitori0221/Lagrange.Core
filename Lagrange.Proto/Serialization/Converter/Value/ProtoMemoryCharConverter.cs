using System.Buffers;
using System.Text.Unicode;
using Lagrange.Proto.Primitives;
using Lagrange.Proto.Utility;

namespace Lagrange.Proto.Serialization.Converter;

public class ProtoMemoryCharConverter : ProtoConverter<Memory<char>>
{
    public override void Write(int field, WireType wireType, ProtoWriter writer, Memory<char> value)
    {
        writer.EncodeString(value.Span);
    }

    public override int Measure(int field, WireType wireType, Memory<char> value)
    {
        return ProtoHelper.CountString(value.Span);
    }

    public override Memory<char> Read(int field, WireType wireType, ref ProtoReader reader)
    {
        int length = reader.DecodeVarInt<int>();
        if (length == 0) return Memory<char>.Empty;
        
        var buffer = ArrayPool<byte>.Shared.Rent(length);
        var utf16 = GC.AllocateUninitializedArray<char>(length);
        var span = reader.CreateSpan(length);
        span.CopyTo(buffer);
        
        Utf8.ToUtf16(buffer, utf16, out _, out int charsWritten);
        ArrayPool<byte>.Shared.Return(buffer);
        
        return new Memory<char>(utf16, 0, charsWritten);
    }
}