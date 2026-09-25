namespace Lagrange.Proto.Nodes;

public partial class ProtoNode
{
    public static implicit operator ProtoNode(bool value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(sbyte value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(byte value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(short value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(ushort value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(int value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(uint value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(long value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(ulong value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(float value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(double value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(bool? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(sbyte? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(byte? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(short? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(ushort? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(int? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(uint? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(long? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(ulong? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(float? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(double? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(string? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(byte[]? value) => value == null ? null! : ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(ReadOnlyMemory<char> value) => ProtoValue.Create(value);
    
    public static implicit operator ProtoNode(ReadOnlyMemory<byte> value) => ProtoValue.Create(value);
    
    public static explicit operator bool(ProtoNode value) => value.GetValue<bool>();
    
    public static explicit operator sbyte(ProtoNode value) => value.GetValue<sbyte>();
    
    public static explicit operator byte(ProtoNode value) => value.GetValue<byte>();
    
    public static explicit operator short(ProtoNode value) => value.GetValue<short>();
    
    public static explicit operator ushort(ProtoNode value) => value.GetValue<ushort>();
    
    public static explicit operator int(ProtoNode value) => value.GetValue<int>();
    
    public static explicit operator uint(ProtoNode value) => value.GetValue<uint>();
    
    public static explicit operator long(ProtoNode value) => value.GetValue<long>();
    
    public static explicit operator ulong(ProtoNode value) => value.GetValue<ulong>();
    
    public static explicit operator float(ProtoNode value) => value.GetValue<float>();
    
    public static explicit operator double(ProtoNode value) => value.GetValue<double>();
    
    public static explicit operator string(ProtoNode value) => value.GetValue<string>();
    
    public static explicit operator byte[](ProtoNode value) => value.GetValue<byte[]>();
}