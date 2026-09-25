using Lagrange.Proto.Serialization.Converter;

namespace Lagrange.Proto.Serialization.Metadata;

public static partial class ProtoTypeResolver
{
    private static void RegisterWellKnownTypes()
    {
        Register(new ProtoBooleanConverter()); 

        Register(new ProtoNumberConverter<SByte>());
        Register(new ProtoNumberConverter<Byte>());
        Register(new ProtoNumberConverter<Int16>());
        Register(new ProtoNumberConverter<UInt16>());
        Register(new ProtoNumberConverter<Int32>());
        Register(new ProtoNumberConverter<UInt32>());
        Register(new ProtoNumberConverter<Int64>());
        Register(new ProtoNumberConverter<UInt64>());
        Register(new ProtoNumberConverter<Single>());
        Register(new ProtoNumberConverter<Double>());

        Register(new ProtoNullableConverter<SByte>());
        Register(new ProtoNullableConverter<Byte>());
        Register(new ProtoNullableConverter<Int16>());
        Register(new ProtoNullableConverter<UInt16>());
        Register(new ProtoNullableConverter<Int32>());
        Register(new ProtoNullableConverter<UInt32>());
        Register(new ProtoNullableConverter<Int64>());
        Register(new ProtoNullableConverter<UInt64>());
        Register(new ProtoNullableConverter<Single>());
        Register(new ProtoNullableConverter<Double>());
        Register(new ProtoNullableConverter<bool>());

        Register(new ProtoStringConverter());
        Register(new ProtoBytesConverter());
        Register(new ProtoReadOnlyMemoryByteConverter());
        Register(new ProtoReadOnlyMemoryCharConverter());
        Register(new ProtoMemoryByteConverter());
        Register(new ProtoMemoryCharConverter());

        Register(new ProtoRawValueConverter());
        Register(new ProtoValueConverter());
        Register(new ProtoArrayConverter());
        Register(new ProtoObjectConverter());
        Register(new ProtoNodeConverter());
    }
}