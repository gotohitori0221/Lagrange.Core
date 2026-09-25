using Lagrange.Proto.Primitives;
using Lagrange.Proto.Serialization.Metadata;

namespace Lagrange.Proto.Serialization.Converter;

public class ProtoNullableConverter<T> : ProtoConverter<T?> where T : struct
{
    private readonly ProtoConverter<T> _converter = ProtoTypeResolver.GetConverter<T>();

    public override void Write(int field, WireType wireType, ProtoWriter writer, T? value)
    {
        if (value.HasValue) _converter.Write(field, wireType, writer, value.Value);
    }
    
    public override void WriteWithNumberHandling(int field, WireType wireType, ProtoWriter writer, T? value, ProtoNumberHandling numberHandling)
    {
        if (value.HasValue) _converter.WriteWithNumberHandling(field, wireType, writer, value.Value, numberHandling);
    }

    public override int Measure(int field, WireType wireType, T? value)
    {
        return value.HasValue ? _converter.Measure(field, wireType, value.Value) : 0;
    }
    
    public override int MeasureWithNumberHandling(int field, WireType wireType, T? value, ProtoNumberHandling numberHandling)
    {
        return value.HasValue ? _converter.MeasureWithNumberHandling(field, wireType, value.Value, numberHandling) : 0;
    }

    public override T? Read(int field, WireType wireType, ref ProtoReader reader)
    {
        return _converter.Read(field, wireType, ref reader);
    }
    
    public override T? ReadWithNumberHandling(int field, WireType wireType, ref ProtoReader reader, ProtoNumberHandling numberHandling)
    {
        return _converter.ReadWithNumberHandling(field, wireType, ref reader, numberHandling);
    }
}