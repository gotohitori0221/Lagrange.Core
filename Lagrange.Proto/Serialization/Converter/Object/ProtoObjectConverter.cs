using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Lagrange.Proto.Primitives;
using Lagrange.Proto.Serialization.Metadata;
using Lagrange.Proto.Utility;

namespace Lagrange.Proto.Serialization.Converter;

[RequiresUnreferencedCode(ProtoSerializer.SerializationUnreferencedCodeMessage)]
[RequiresDynamicCode(ProtoSerializer.SerializationRequiresDynamicCodeMessage)]
internal class ProtoObjectConverter<T>(ProtoObjectInfo<T> objectInfo) : ProtoConverter<T>
{
    internal readonly ProtoObjectInfo<T> ObjectInfo = objectInfo;
    
    internal ProtoObjectConverter() : this(ProtoTypeResolver.CreateObjectInfo<T>()) { }

    public override void Write(int field, WireType wireType, ProtoWriter writer, T value)
    {
        object? boxed = value; 
        if (boxed is null) return;
        
        int length = Measure(field, wireType, value);
        writer.EncodeVarInt(length);
        
        foreach (var (tag, info) in ObjectInfo.Fields)
        {
            writer.EncodeVarInt(tag);
            info.Write(writer, boxed);
        }
    }

    public override int Measure(int field, WireType wireType, T value)
    {
        object? boxed = value; 
        if (boxed is null) return 0;
        
        int result = 0;
        foreach (var (tag, info) in ObjectInfo.Fields)
        {
            result += ProtoHelper.GetVarIntLength(tag);
            result += info.Measure(boxed);
        }
        
        return result;
    }

    public override T Read(int field, WireType wireType, ref ProtoReader reader)
    {
        Debug.Assert(ObjectInfo.ObjectCreator != null);
        
        T target = ObjectInfo.ObjectCreator();
        var boxed = (object?)target; 
        if (boxed is null) ThrowHelper.ThrowInvalidOperationException_CanNotCreateObject(typeof(T));

        int length = reader.DecodeVarInt<int>();
        var subSpan = reader.CreateSpan(length);
        var subReader = new ProtoReader(subSpan);
        while (!subReader.IsCompleted)
        {
            uint tag = subReader.DecodeVarIntUnsafe<uint>();
            if (ObjectInfo.Fields.TryGetValue(tag, out var fieldInfo))
            {
                fieldInfo.Read(ref subReader, boxed);
            }
            else
            {
                subReader.SkipField((WireType)(tag & 0x07));
            }
        }
        
        return target;
    }
}