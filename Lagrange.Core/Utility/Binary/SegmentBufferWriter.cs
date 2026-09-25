using System.Buffers;
using System.Runtime.CompilerServices;

namespace Lagrange.Core.Utility.Binary;




internal sealed class SegmentBufferWriter : IBufferWriter<byte>, IDisposable
{
    private const int DefaultSegmentSize = 2048;
    
    private int _position;
    private int _bytesWritten;
    private byte[] _currentSegment = ArrayPool<byte>.Shared.Rent(DefaultSegmentSize);

    private readonly List<byte[]> _cachedSegments = [];
    private readonly List<CompletedBuffer> _completedBuffers = [];

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

    public void WriteTo(ref BinaryPacket packet)
    {
        var span = packet.CreateSpan(_bytesWritten);
        foreach (var buffer in _completedBuffers)
        {
            buffer.Span.CopyTo(span);
            span = span[buffer.Length..];
        }
        
        _currentSegment.AsSpan(0, _position).CopyTo(span);
    }
    
    public ReadOnlyMemory<byte> CreateReadOnlyMemory()
    {
        var result = new byte[_bytesWritten];
        var span = result.AsSpan();
        
        foreach (var buffer in _completedBuffers)
        {
            buffer.Span.CopyTo(span);
            span = span[buffer.Length..];
        }
        
        _currentSegment.AsSpan(0, _position).CopyTo(span);

        return new ReadOnlyMemory<byte>(result);
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
        _position = 0;
    }
    
    
    
    
    private readonly struct CompletedBuffer(byte[] buffer, int length)
    {
        public byte[] Buffer { get; } = buffer;
        public int Length { get; } = length;

        public ReadOnlySpan<byte> Span => Buffer.AsSpan(0, Length);

        public void Return() => ArrayPool<byte>.Shared.Return(Buffer);
    }
}