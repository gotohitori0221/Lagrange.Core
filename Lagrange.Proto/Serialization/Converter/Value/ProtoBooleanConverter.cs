using System.Runtime.CompilerServices;
using Lagrange.Proto.Primitives;

namespace Lagrange.Proto.Serialization.Converter;

internal class ProtoBooleanConverter : ProtoConverter<bool>
{
    public override void Write(int field, WireType wireType, ProtoWriter writer, bool value)
    {
        writer.WriteRawByte(Unsafe.As<bool, byte>(ref value));
    }

    public override int Measure(int field, WireType wireType, bool value)
    {
        return 1;
    }

    public override bool Read(int field, WireType wireType, ref ProtoReader reader)
    {
        byte b = reader.DecodeVarInt<byte>();
        return Unsafe.As<byte, bool>(ref b);
    }
}