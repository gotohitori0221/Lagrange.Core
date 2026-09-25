using System.Buffers.Binary;
using System.Security.Cryptography;

namespace Lagrange.Core.Utility.Cryptography;

internal static class TriSha1Provider
{
    private const int Sha1SampleSize = 30 * 1024 * 1024;
    
    private const int SingleSampleSize = 10 * 1024 * 1024;
    
    public static byte[] CalculateTriSha1(Stream stream)
    {
        if (!stream.CanSeek) throw new ArgumentException("Stream must support seeking/Length.", nameof(stream));

        long length = stream.Length;
        byte[] sample;

        switch (length)
        {
            case <= Sha1SampleSize:
                sample = GC.AllocateUninitializedArray<byte>((int)length + sizeof(long));
                stream.Position = 0;
                stream.ReadExactly(sample.AsSpan(0, (int)length));
                BinaryPrimitives.WriteInt64LittleEndian(sample.AsSpan((int)length), length);
                break;
            default:
                sample = GC.AllocateUninitializedArray<byte>(Sha1SampleSize + sizeof(long));

                stream.Position = 0;
                stream.ReadExactly(sample.AsSpan(0, SingleSampleSize));

                stream.Position = length / 2 - SingleSampleSize / 2;
                stream.ReadExactly(sample.AsSpan(SingleSampleSize, SingleSampleSize));

                stream.Position = length - SingleSampleSize;
                stream.ReadExactly(sample.AsSpan(SingleSampleSize * 2, SingleSampleSize));
                BinaryPrimitives.WriteInt64LittleEndian(sample.AsSpan(Sha1SampleSize), length);
                break;
        }

        stream.Position = 0;
        return SHA1.HashData(sample);
    }
    
    public static byte[] CalculateTriSha1(ReadOnlySpan<byte> data)
    {
        byte[] sample;
        
        switch (data.Length)
        {
            case <= Sha1SampleSize:
                sample = GC.AllocateUninitializedArray<byte>(data.Length + sizeof(long));
                data.CopyTo(sample);
                BinaryPrimitives.WriteInt64LittleEndian(sample.AsSpan(data.Length), data.Length);
                break;
            default:
                sample = GC.AllocateUninitializedArray<byte>(Sha1SampleSize + sizeof(long));
                data.Slice(0, SingleSampleSize).CopyTo(sample);
                BinaryPrimitives.WriteInt64LittleEndian(sample.AsSpan(Sha1SampleSize), data.Length);
                break;
        }

        return SHA1.HashData(sample);
    }
}