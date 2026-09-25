using Lagrange.Proto.Primitives;
using Lagrange.Proto.Serialization.Metadata;
using Lagrange.Proto.Utility;

namespace Lagrange.Proto.Serialization.Converter;

public class ProtoSerializableConverter<T> : ProtoConverter<T> where T : IProtoSerializable<T>
{
    private readonly ProtoObjectInfo<T> _object = T.TypeInfo; 
    
    public override void Write(int field, WireType wireType, ProtoWriter writer, T value)
    {
        int length = T.MeasureHandler(value);
        writer.EncodeVarInt(length);
        if (length > 0) T.SerializeHandler(value, writer);
    }

    public override int Measure(int field, WireType wireType, T value)
    {
        int length = T.MeasureHandler(value);
        return ProtoHelper.GetVarIntLength(length) + length;
    }

    public override T Read(int field, WireType wireType, ref ProtoReader reader)
    {
        int length = reader.DecodeVarInt<int>();
        var span = reader.CreateSpan(length);
        return ProtoSerializer.DeserializeProtoPackable<T>(span);
    }
}