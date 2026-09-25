using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Lagrange.Proto.Primitives;
using Lagrange.Proto.Serialization.Converter;
using Lagrange.Proto.Serialization.Metadata;

namespace Lagrange.Proto.Serialization;




public static partial class ProtoSerializer
{
    
    
    
    
    
    
    public static void SerializeProtoPackable<T>(IBufferWriter<byte> dest, T obj) where T : IProtoSerializable<T>
    {
        var writer = ProtoWriterCache.RentWriter(dest);
        try
        {
            SerializeProtoPackableCore(writer, obj);
        }
        finally
        {
            ProtoWriterCache.ReturnWriter(writer);
        }
    }
    
    
    
    
    
    
    public static byte[] SerializeProtoPackable<T>(T obj) where T : IProtoSerializable<T>
    {
        var writer = ProtoWriterCache.RentWriterAndBuffer(512, out var buffer);
        try
        {
            SerializeProtoPackableCore(writer, obj);
            return buffer.ToArray();
        }
        finally
        {
            ProtoWriterCache.ReturnWriterAndBuffer(writer, buffer);
        }
    }
    
    private static void SerializeProtoPackableCore<T>(ProtoWriter writer, T obj) where T : IProtoSerializable<T>
    {
        if (!ProtoTypeResolver.IsRegistered<T>()) ProtoTypeResolver.Register(new ProtoSerializableConverter<T>());

        T.SerializeHandler(obj, writer);
        writer.Flush();
    }
    
    
    
    
    
    
    
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
    public static void Serialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(IBufferWriter<byte> dest, T obj) 
    {
        var writer = ProtoWriterCache.RentWriter(dest);
        try
        {
            SerializeCore(writer, obj);
        }
        finally
        {
            ProtoWriterCache.ReturnWriter(writer);
        }
    }
    
    
    
    
    
    
    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
    public static byte[] Serialize<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(T obj) 
    {
        var writer = ProtoWriterCache.RentWriterAndBuffer(512, out var buffer);
        try
        {
            SerializeCore(writer, obj);
            return buffer.ToArray();
        }
        finally
        {
            ProtoWriterCache.ReturnWriterAndBuffer(writer, buffer);
        }
    }

    [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
    private static void SerializeCore<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(ProtoWriter writer, T obj)
    {       
        ProtoObjectConverter<T> converter;
        if (ProtoTypeResolver.IsRegistered<T>())
        {
            if (ProtoTypeResolver.GetConverter<T>() as ProtoObjectConverter<T> is not { } c)
            {
                converter = new ProtoObjectConverter<T>(ProtoTypeResolver.CreateObjectInfo<T>());
                ProtoTypeResolver.Register(converter);
            }
            else
            {
                converter = c;
            }
        }
        else
        {
            ProtoTypeResolver.Register(converter = new ProtoObjectConverter<T>());
        }
        
        var objectInfo = converter.ObjectInfo;
        object? boxed = obj; 
        if (boxed is null) return;
        
        foreach (var (tag, info) in objectInfo.Fields)
        {
            if (info.ShouldSerialize(boxed, objectInfo.IgnoreDefaultFields))
            {
                writer.EncodeVarInt(tag);
                info.Write(writer, boxed);
            }
        }
        writer.Flush();
    }
}