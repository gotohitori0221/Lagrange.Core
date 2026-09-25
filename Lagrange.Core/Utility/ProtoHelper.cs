using System.Collections.Concurrent;
using Lagrange.Core.Utility.Binary;
using Lagrange.Proto;
using Lagrange.Proto.Serialization;

namespace Lagrange.Core.Utility;

internal static class ProtoHelper
{
    private static readonly ConcurrentQueue<SegmentBufferWriter> BufferPool = new();
    
    public static void Serialize<T>(ref BinaryPacket dest, T value) where T : IProtoSerializable<T>
    {
        if (!BufferPool.TryDequeue(out var writer))
        {
            writer = new SegmentBufferWriter();
        }
        
        ProtoSerializer.SerializeProtoPackable(writer, value);
        writer.WriteTo(ref dest);
        writer.Clear();
        
        BufferPool.Enqueue(writer);
    }
    
    public static ReadOnlyMemory<byte> Serialize<T>(T value) where T : IProtoSerializable<T>
    {
        if (!BufferPool.TryDequeue(out var writer))
        {
            writer = new SegmentBufferWriter();
        }
        
        ProtoSerializer.SerializeProtoPackable(writer, value);
        var result = writer.CreateReadOnlyMemory();
        writer.Clear();
        BufferPool.Enqueue(writer);
        
        return result;
    }

    public static T Deserialize<T>(ReadOnlySpan<byte> src)  where T : IProtoSerializable<T> => ProtoSerializer.DeserializeProtoPackable<T>(src);
}