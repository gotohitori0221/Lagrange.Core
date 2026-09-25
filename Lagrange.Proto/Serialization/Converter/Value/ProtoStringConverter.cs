using System.Text;
using Lagrange.Proto.Primitives;
using Lagrange.Proto.Utility;

namespace Lagrange.Proto.Serialization.Converter;

internal class ProtoStringConverter : ProtoConverter<string>
{
    public override void Write(int field, WireType wireType, ProtoWriter writer, string value)
    { 
        writer.EncodeString(value);
    }

    public override int Measure(int field, WireType wireType, string value)
    {
        return ProtoHelper.CountString(value);
    }

    public override string Read(int field, WireType wireType, ref ProtoReader reader)
    {
        int length = reader.DecodeVarInt<int>();
        var span = reader.CreateSpan(length);
        return span.IsEmpty ? string.Empty : Encoding.UTF8.GetString(span);
    }
}