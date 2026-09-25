using Lagrange.Proto.Serialization;

namespace Lagrange.Proto.Nodes;

public partial class ProtoValue
{
    public static ProtoValue Create(bool value) => new ProtoValue<bool>(value, WireType.VarInt);

    public static ProtoValue Create(sbyte value) => new ProtoValue<sbyte>(value, WireType.VarInt);
    
    public static ProtoValue Create(byte value) => new ProtoValue<byte>(value, WireType.VarInt);
    
    public static ProtoValue Create(short value) => new ProtoValue<short>(value, WireType.VarInt);
    
    public static ProtoValue Create(ushort value) => new ProtoValue<ushort>(value, WireType.VarInt);
    
    public static ProtoValue Create(int value) => new ProtoValue<int>(value, WireType.VarInt);
    
    public static ProtoValue Create(uint value) => new ProtoValue<uint>(value, WireType.VarInt);
    
    public static ProtoValue Create(long value) => new ProtoValue<long>(value, WireType.VarInt);
    
    public static ProtoValue Create(ulong value) => new ProtoValue<ulong>(value, WireType.VarInt);
    
    public static ProtoValue Create(float value) => new ProtoValue<float>(value, WireType.Fixed32);
    
    public static ProtoValue Create(double value) => new ProtoValue<double>(value, WireType.Fixed64);
    
    public static ProtoValue Create(bool? value) => new ProtoValue<bool?>(value, WireType.VarInt);
    
    public static ProtoValue Create(sbyte? value) => new ProtoValue<sbyte?>(value, WireType.VarInt);
    
    public static ProtoValue Create(byte? value) => new ProtoValue<byte?>(value, WireType.VarInt);
    
    public static ProtoValue Create(short? value) => new ProtoValue<short?>(value, WireType.VarInt);
    
    public static ProtoValue Create(ushort? value) => new ProtoValue<ushort?>(value, WireType.VarInt);
    
    public static ProtoValue Create(int? value) => new ProtoValue<int?>(value, WireType.VarInt);
    
    public static ProtoValue Create(uint? value) => new ProtoValue<uint?>(value, WireType.VarInt);
    
    public static ProtoValue Create(long? value) => new ProtoValue<long?>(value, WireType.VarInt);
    
    public static ProtoValue Create(ulong? value) => new ProtoValue<ulong?>(value, WireType.VarInt);
    
    public static ProtoValue Create(float? value) => new ProtoValue<float?>(value, WireType.Fixed32);
    
    public static ProtoValue Create(double? value) => new ProtoValue<double?>(value, WireType.Fixed64);
    
    public static ProtoValue Create(string value) => new ProtoValue<string>(value, WireType.LengthDelimited);
    
    public static ProtoValue Create(byte[] value) => new ProtoValue<byte[]>(value, WireType.LengthDelimited);
    
    public static ProtoValue Create(ReadOnlyMemory<byte> value) => new ProtoValue<ReadOnlyMemory<byte>>(value, WireType.LengthDelimited);
    
    public static ProtoValue Create(ReadOnlyMemory<char> value) => new ProtoValue<ReadOnlyMemory<char>>(value, WireType.LengthDelimited);

    public static ProtoValue Create<T>(T value) where T : IProtoSerializable<T> => new ProtoValue<T>(value, WireType.LengthDelimited);
}