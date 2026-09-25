using System.Numerics;
using Lagrange.Proto.Primitives;
using Lagrange.Proto.Utility;

namespace Lagrange.Proto.Serialization.Converter;

internal class ProtoNumberConverter<T> : ProtoConverter<T> where T : unmanaged, INumber<T>
{
    public override bool ShouldSerialize(T value, bool ignoreDefaultValue)
    {
        return !ignoreDefaultValue || value != default;
    }
    
    public override void Write(int field, WireType wireType, ProtoWriter writer, T value)
    {
        switch (wireType)
        {
            case WireType.Fixed32:
                writer.EncodeFixed32(value);
                break;
            case WireType.Fixed64:
                writer.EncodeFixed64(value);
                break;
            case WireType.VarInt:
                writer.EncodeVarInt(value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(wireType), wireType, null);
        }
    }

    public override unsafe void WriteWithNumberHandling(int field, WireType wireType, ProtoWriter writer, T value, ProtoNumberHandling numberHandling)
    {
        if ((numberHandling & ProtoNumberHandling.Signed) != 0)
        {
            var encoded = ProtoHelper.ZigZagEncode(value);
            if (wireType == WireType.VarInt)
            {
                switch (sizeof(T))
                {
                    case 1:
                        writer.EncodeVarInt(byte.CreateSaturating(encoded));
                        break;
                    case 2:
                        writer.EncodeVarInt(ushort.CreateSaturating(encoded));
                        break;
                    case 4:
                        writer.EncodeVarInt(uint.CreateSaturating(encoded));
                        break;
                    case 8:
                        writer.EncodeVarInt(ulong.CreateSaturating(encoded));
                        break;
                    default:
                        Write(field, wireType, writer, encoded);
                        break;
                }
            }
            else
            {
                Write(field, wireType, writer, encoded);
            }
        }
        else
        {
            Write(field, wireType, writer, value);
        }
    }

    public override int Measure(int field, WireType wireType, T value)
    {
        return wireType switch
        {
            WireType.Fixed32 => sizeof(float),
            WireType.Fixed64 => sizeof(double),
            WireType.VarInt => ProtoHelper.GetVarIntLength(value),
            _ => throw new ArgumentOutOfRangeException(nameof(wireType), wireType, null)
        };
    }
    
    public override unsafe int MeasureWithNumberHandling(int field, WireType wireType, T value, ProtoNumberHandling numberHandling)
    {
        if ((numberHandling & ProtoNumberHandling.Signed) != 0 && wireType == WireType.VarInt)
        {
            T encoded = ProtoHelper.ZigZagEncode(value);
            return sizeof(T) switch
            {
                1 => ProtoHelper.GetVarIntLength(byte.CreateSaturating(encoded)),
                2 => ProtoHelper.GetVarIntLength(ushort.CreateSaturating(encoded)),
                4 => ProtoHelper.GetVarIntLength(uint.CreateSaturating(encoded)),
                8 => ProtoHelper.GetVarIntLength(ulong.CreateSaturating(encoded)),
                _ => ProtoHelper.GetVarIntLength(encoded)
            };
        }
        return Measure(field, wireType, value);
    }

    public override T Read(int field, WireType wireType, ref ProtoReader reader)
    {
        return wireType switch
        {
            WireType.Fixed32 => reader.DecodeFixed32<T>(),
            WireType.Fixed64 => reader.DecodeFixed64<T>(),
            WireType.VarInt => reader.DecodeVarInt<T>(),
            _ => throw new ArgumentOutOfRangeException(nameof(wireType), wireType, null)
        };
    }
    
    public override unsafe T ReadWithNumberHandling(int field, WireType wireType, ref ProtoReader reader, ProtoNumberHandling numberHandling)
    {
        if ((numberHandling & ProtoNumberHandling.Signed) != 0 && wireType == WireType.VarInt)
        {
            T value = sizeof(T) switch
            {
                1 => T.CreateTruncating(reader.DecodeVarInt<byte>()),
                2 => T.CreateTruncating(reader.DecodeVarInt<ushort>()),
                4 => T.CreateTruncating(reader.DecodeVarInt<uint>()),
                8 => T.CreateTruncating(reader.DecodeVarInt<ulong>()),
                _ => Read(field, wireType, ref reader)
            };
            return ProtoHelper.ZigZagDecode(value);
        }
        else
        {
            T value = Read(field, wireType, ref reader);
            return (numberHandling & ProtoNumberHandling.Signed) != 0 ? ProtoHelper.ZigZagDecode(value) : value;
        }
    }
}