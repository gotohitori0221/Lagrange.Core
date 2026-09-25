using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Lagrange.Proto.Primitives;
using Lagrange.Proto.Serialization;
using Lagrange.Proto.Serialization.Metadata;
using Lagrange.Proto.Utility;

namespace Lagrange.Proto.Nodes;

public abstract partial class ProtoValue(WireType wireType) : ProtoNode(wireType)
{
    public abstract bool TryGetValue<T>([NotNullWhen(true)] out T? value);
}

public sealed class ProtoValue<TValue> : ProtoValue
{
    internal readonly TValue Value; 

    private readonly ProtoConverter<TValue> _converter;
    
    public ProtoValue(TValue value, WireType wireType) : base(wireType)
    {
        Debug.Assert(value != null || (typeof(TValue).IsGenericType && typeof(TValue).GetGenericTypeDefinition() == typeof(Nullable<>)));
        Debug.Assert(value is not ProtoNode);
        
        Value = value;
        _converter = ProtoTypeResolver.GetConverter<TValue>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override T GetValue<T>()
    {
        if (Value is ProtoRawValue rawValue)
        {
            if (WireType is not WireType.LengthDelimited)
            {
                if (WireType is not (WireType.Fixed32 or WireType.Fixed64))
                {
                    
                    if (typeof(T) == typeof(ulong))
                    {
                        return (T)(object)(ulong)rawValue.Value;
                    }
                    if (typeof(T) == typeof(uint))
                    {
                        return (T)(object)(uint)rawValue.Value;
                    }
                    if (typeof(T) == typeof(ushort))
                    {
                        return (T)(object)(ushort)rawValue.Value;
                    }
                    return (T)Convert.ChangeType(rawValue.Value, typeof(T));
                }

                long value = rawValue.Value;
                return Unsafe.As<long, T>(ref value);
            }

            if (typeof(T) == typeof(string)) return (T)(object)Encoding.UTF8.GetString(rawValue.Bytes.Span);
            if (typeof(T) == typeof(byte[])) return (T)(object)rawValue.Bytes.ToArray();
        }
        
        if (Value is T t) return t;
        
        
        if (Value != null && IsNumericType(Value.GetType()) && IsNumericType(typeof(T)))
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }

        ThrowHelper.ThrowInvalidOperationException_InvalidWireType(WireType);
        return default!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool TryGetValue<T>([NotNullWhen(true)] out T? value) where T : default
    {
        value = default;
        
        if (Value is ProtoRawValue rawValue)
        {
            if (WireType is WireType.LengthDelimited)
            {
                if (typeof(T) == typeof(string))
                {
                    value = (T)(object)Encoding.UTF8.GetString(rawValue.Bytes.Span)!;
                    return true;
                }
                if (typeof(T) == typeof(byte[]))
                {
                    value = (T)(object)rawValue.Bytes.ToArray()!;
                    return true;
                }
            }
            else
            {
                if (WireType is WireType.Fixed32 or WireType.Fixed64)
                {
                    long val = rawValue.Value;
                    value = Unsafe.As<long, T>(ref val)!;
                    return true;
                }
                else
                {
                    try
                    {
                        value = (T)Convert.ChangeType(rawValue.Value, typeof(T))!;
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
        
        if (Value is T t)
        {
            value = t;
            return true;
        }
        
        
        if (Value != null && IsNumericType(Value.GetType()) && IsNumericType(typeof(T)))
        {
            try
            {
                value = (T)Convert.ChangeType(Value, typeof(T))!;
                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

    private protected override ProtoNode GetItem(int field)
    {
        return this is ProtoValue<ProtoRawValue> && WireType is WireType.LengthDelimited
            ? AsObject()[field]
            : base.GetItem(field);
    }
    
    public override void WriteTo(int field, ProtoWriter writer)
    {
        _converter.Write(field, WireType, writer, Value);
    }

    private static bool IsNumericType(Type type)
    {
        return type == typeof(byte) || type == typeof(sbyte) ||
               type == typeof(short) || type == typeof(ushort) ||
               type == typeof(int) || type == typeof(uint) ||
               type == typeof(long) || type == typeof(ulong) ||
               type == typeof(float) || type == typeof(double) ||
               type == typeof(decimal);
    }
    
    public override int Measure(int field)
    {
        return Value is ProtoRawValue rawValue
            ? WireType switch
            {
                WireType.LengthDelimited => rawValue.Bytes.Length,
                WireType.Fixed32 => 4,
                WireType.Fixed64 => 8,
                WireType.VarInt => ProtoHelper.GetVarIntLength(rawValue.Value),
                _ => throw new InvalidOperationException($"Invalid wire type {WireType} for the node.")
            }
            : _converter.Measure(field, WireType, Value);
    }
}