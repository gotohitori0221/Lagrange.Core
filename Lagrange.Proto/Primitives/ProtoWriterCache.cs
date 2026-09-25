using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using Lagrange.Proto.Utility;

namespace Lagrange.Proto.Primitives;




internal static class ProtoWriterCache
{
    [ThreadStatic]
    private static ThreadLocalState? _threadLocalState;
    
    public static ProtoWriter RentWriter(IBufferWriter<byte> bufferWriter)
    {
        var state = _threadLocalState ??= new ThreadLocalState();
        ProtoWriter writer;

        if (state.RentedWriters++ == 0)
        {
            writer = state.Writer;
            writer.Reset(bufferWriter);
        }
        else
        {
            writer = new ProtoWriter(bufferWriter);
        }

        return writer;
    }
    
    public static ProtoWriter RentWriterAndBuffer(int defaultBufferSize, out SegmentBufferWriter bufferWriter)
    {
        var state = _threadLocalState ??= new ThreadLocalState();
        ProtoWriter writer;

        if (state.RentedWriters++ == 0)
        {
            bufferWriter = state.BufferWriter;
            writer = state.Writer;

            bufferWriter.InitializeEmptyInstance(defaultBufferSize);
            writer.Reset(bufferWriter);
        }
        else
        {
            bufferWriter = new SegmentBufferWriter(defaultBufferSize);
            writer = new ProtoWriter(bufferWriter);
        }

        return writer;
    }

    public static void ReturnWriter(ProtoWriter writer)
    {
        Debug.Assert(_threadLocalState != null);
            
        writer.ResetAllStateForCacheReuse();

        int rentedWriters = --_threadLocalState.RentedWriters;
        Debug.Assert((rentedWriters == 0) == ReferenceEquals(_threadLocalState.Writer, writer));
    }
    
    public static void ReturnWriterAndBuffer(ProtoWriter writer, SegmentBufferWriter bufferWriter)
    {
        Debug.Assert(_threadLocalState != null);
        var state = _threadLocalState;

        writer.ResetAllStateForCacheReuse();
        bufferWriter.Clear();

        int rentedWriters = --state.RentedWriters;
        Debug.Assert((rentedWriters == 0) == (ReferenceEquals(state.BufferWriter, bufferWriter) && ReferenceEquals(state.Writer, writer)));
    }
    
    private sealed class ThreadLocalState
    {
        public readonly ProtoWriter Writer = new();
        public readonly SegmentBufferWriter BufferWriter = new();
        public int RentedWriters;
    }
}