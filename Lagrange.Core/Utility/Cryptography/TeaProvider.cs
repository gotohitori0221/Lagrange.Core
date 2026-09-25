using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Lagrange.Core.Utility.Cryptography;

internal static class TeaProvider
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetCipherLength(int data) => (10 - ((data + 1) & 7)) + data + 7;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetPlainLength(int data) => data - ((data & 7) + 3) - 7;
    
    public static byte[] Encrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
    {
        var dest = new byte[GetCipherLength(data.Length)];
        Encrypt(data, dest.AsSpan(), key);
        return dest;
    }

    public static int Encrypt(ReadOnlySpan<byte> source, Span<byte> dest, ReadOnlySpan<byte> key)
    {
        int fill = 10 - ((source.Length + 1) & 7);
        int length = fill + source.Length + 7;
        Debug.Assert(dest.Length >= fill + source.Length + 7, "Destination buffer is too small.");

        uint a = BinaryPrimitives.ReadUInt32BigEndian(key[..]);
        uint b = BinaryPrimitives.ReadUInt32BigEndian(key[4..]);
        uint c = BinaryPrimitives.ReadUInt32BigEndian(key[8..]);
        uint d = BinaryPrimitives.ReadUInt32BigEndian(key[12..]);
        
        Random.Shared.NextBytes(dest[..fill]);
        dest[0] = (byte)((fill - 3) | 0xF8);
        source.CopyTo(dest[fill..]);
        
        ulong plainXor = 0, prevXor = 0;
        
        for (int i = 0; i < length; i += 8)
        {
            ulong plain = BinaryPrimitives.ReadUInt64BigEndian(dest[i..]) ^ plainXor;
            uint x = (uint)(plain >> 32);
            uint y = (uint)(plain);

            x += (y + 0x9e3779b9) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x9e3779b9) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0x3c6ef372) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x3c6ef372) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0xdaa66d2b) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0xdaa66d2b) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0x78dde6e4) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x78dde6e4) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0x1715609d) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x1715609d) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0xb54cda56) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0xb54cda56) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0x5384540f) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x5384540f) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0xf1bbcdc8) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0xf1bbcdc8) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0x8ff34781) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x8ff34781) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0x2e2ac13a) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x2e2ac13a) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0xcc623af3) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0xcc623af3) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0x6a99b4ac) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x6a99b4ac) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0x08d12e65) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x08d12e65) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0xa708a81e) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0xa708a81e) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0x454021d7) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0x454021d7) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x += (y + 0xe3779b90) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y += (x + 0xe3779b90) ^ ((x << 4) + c) ^ ((x >> 5) + d);

            plainXor = ((ulong)x << 32 | y) ^ prevXor;
            prevXor = plain;
            BinaryPrimitives.WriteUInt64BigEndian(dest[i..], plainXor);
        }
        
        return length;
    }

    public static byte[] Decrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key)
    {
        var dest = new byte[data.Length];
        Decrypt(data, dest.AsSpan(), key);
        return dest[((dest[0] & 7) + 3)..^7];
    }

    public static int Decrypt(ReadOnlySpan<byte> source, Span<byte> dest, ReadOnlySpan<byte> key)
    {
        Debug.Assert(dest.Length >= GetPlainLength(source.Length), "Destination buffer is too small.");
        
        uint a = BinaryPrimitives.ReadUInt32BigEndian(key[..]);
        uint b = BinaryPrimitives.ReadUInt32BigEndian(key[4..]);
        uint c = BinaryPrimitives.ReadUInt32BigEndian(key[8..]);
        uint d = BinaryPrimitives.ReadUInt32BigEndian(key[12..]);

        int fill = (source[0] & 7) + 3;
        ulong plainXor = 0, prevXor = 0;
        for (int i = 0; i < source.Length; i += 8)
        {
            ulong plain = BinaryPrimitives.ReadUInt64BigEndian(source[i..]);
            plainXor ^= plain;
            uint x = (uint)(plainXor >> 32);
            uint y = (uint)(plainXor);

            y -= (x + 0xe3779b90) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0xe3779b90) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x454021d7) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x454021d7) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0xa708a81e) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0xa708a81e) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x08d12e65) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x08d12e65) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x6a99b4ac) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x6a99b4ac) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0xcc623af3) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0xcc623af3) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x2e2ac13a) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x2e2ac13a) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x8ff34781) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x8ff34781) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0xf1bbcdc8) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0xf1bbcdc8) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x5384540f) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x5384540f) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0xb54cda56) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0xb54cda56) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x1715609d) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x1715609d) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x78dde6e4) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x78dde6e4) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0xdaa66d2b) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0xdaa66d2b) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x3c6ef372) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x3c6ef372) ^ ((y << 4) + a) ^ ((y >> 5) + b);
            y -= (x + 0x9e3779b9) ^ ((x << 4) + c) ^ ((x >> 5) + d);
            x -= (y + 0x9e3779b9) ^ ((y << 4) + a) ^ ((y >> 5) + b);

            plainXor = ((ulong)x << 32) | y;
            BinaryPrimitives.WriteUInt64BigEndian(dest[i..], plainXor ^ prevXor);
            prevXor = plain;
        }

        return source.Length - fill - 7;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> CreateDecryptSpan(ReadOnlySpan<byte> decrypted) => decrypted[((decrypted[0] & 7) + 3)..];
}