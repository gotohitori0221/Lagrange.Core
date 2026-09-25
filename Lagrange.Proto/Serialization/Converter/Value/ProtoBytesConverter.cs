using Lagrange.Proto.Primitives;
using Lagrange.Proto.Utility;

namespace Lagrange.Proto.Serialization.Converter;

public class ProtoBytesConverter : ProtoConverter<byte[]>
{
    public override void Write(int field, WireType wireType, ProtoWriter writer, byte[] value)
    {
        writer.EncodeVarInt(value.Length);
        writer.WriteRawBytes(value);
    }

    public override int Measure(int field, WireType wireType, byte[] value)
    {
        return ProtoHelper.CountBytes(value);
    }

    public override byte[] Read(int field, WireType wireType, ref ProtoReader reader)
    {
        int length = reader.DecodeVarInt<int>();
        if (length == 0) return [];

        var buffer = GC.AllocateUninitializedArray<byte>(length);
        var span = reader.CreateSpan(length);
        span.CopyTo(buffer);
        
        return buffer;
    }
}