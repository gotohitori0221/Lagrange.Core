using System.Buffers;
using System.Runtime.CompilerServices;

namespace Lagrange.Proto.Utility;




internal sealed class SegmentBufferWriter : IBufferWriter<byte>, IDisposable
{
    private const int DefaultSegmentSize = 2048;
    
    private int _position;
    private int _bytesWritten;
    private int _totalSize;
    private byte[] _currentSegment;

    private readonly List<byte[]> _cachedSegments = [];
    private readonly List<CompletedBuffer> _completedBuffers = [];

    public SegmentBufferWriter()
    {
        _currentSegment = ArrayPool<byte>.Shared.Rent(DefaultSegmentSize);
    }
    
    public SegmentBufferWriter(int initialSize)
    {
        _currentSegment = ArrayPool<byte>.Shared.Rent(initialSize);
    }

    public void Advance(int count)
    {
        _position += count;
        _bytesWritten += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        if (sizeHint != 0 && _currentSegment.Length - _position < sizeHint) RentSegment(sizeHint);

        return _currentSegment.AsMemory(_position);
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        if (sizeHint != 0 && _currentSegment.Length - _position < sizeHint) RentSegment(sizeHint);
        
        return _currentSegment.AsSpan(_position);
    }

    public void Dispose()
    {
        foreach (var buffer in _completedBuffers) buffer.Return();
        _completedBuffers.Clear();
        
        foreach (var buffer in _cachedSegments) ArrayPool<byte>.Shared.Return(buffer);
        ArrayPool<byte>.Shared.Return(_currentSegment);
    }
    
    public void Clear()
    {
        _position = 0;
        _bytesWritten = 0;

        foreach (var buffer in _completedBuffers) _cachedSegments.Add(buffer.Buffer);
        _completedBuffers.Clear();
    }
    
    public ReadOnlyMemory<byte> CreateReadOnlyMemory()
    {
        var result = GC.AllocateUninitializedArray<byte>(_bytesWritten);
        var span = result.AsSpan();
        
        foreach (var buffer in _completedBuffers)
        {
            buffer.Span.CopyTo(span);
            span = span[buffer.Length..];
        }
        
        _currentSegment.AsSpan(0, _position).CopyTo(span);

        return new ReadOnlyMemory<byte>(result);
    }

    public byte[] ToArray()
    {
        var result = GC.AllocateUninitializedArray<byte>(_bytesWritten);
        var span = result.AsSpan();
        
        foreach (var buffer in _completedBuffers)
        {
            buffer.Span.CopyTo(span);
            span = span[buffer.Length..];
        }
        
        _currentSegment.AsSpan(0, _position).CopyTo(span);

        return result;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void RentSegment(int sizeHint)
    {
        _completedBuffers.Add(new CompletedBuffer(_currentSegment, _position));
        
        foreach (var buffer in _cachedSegments)
        {
            if (buffer.Length >= sizeHint)
            {
                _currentSegment = buffer;
                _position = 0;
                _cachedSegments.Remove(buffer);
                return;
            }
        }
        
        _currentSegment = ArrayPool<byte>.Shared.Rent(sizeHint);
        _totalSize += sizeHint;
        _position = 0;
    }
    
    
    
    
    private readonly struct CompletedBuffer(byte[] buffer, int length)
    {
        public byte[] Buffer { get; } = buffer;
        public int Length { get; } = length;

        public ReadOnlySpan<byte> Span => Buffer.AsSpan(0, Length);

        public void Return() => ArrayPool<byte>.Shared.Return(Buffer);
    }

    public void InitializeEmptyInstance(int size)
    {
        _position = 0;
        _bytesWritten = 0;

        while (size > _totalSize)
        {
            RentSegment(size);
        }
    }
}