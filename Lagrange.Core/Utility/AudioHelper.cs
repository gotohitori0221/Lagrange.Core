using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Lagrange.Core.Utility;

public enum AudioFormat
{
    Unknown,
    Wav,
    Mp3,
    SilkV3,
    TenSilkV3,
    Amr,
    Ogg,
}

internal static class AudioHelper
{
    
    
    
    
    
    
    private const ulong WAV_HEAD_LOWER = 0x5249464600000000UL;
    private const ulong WAV_HEAD_UPPER = 0x57415645666D7400UL;

    
    
    
    
    
    
    private const ulong TENSILKV3_HEAD_LOWER = 0x02232153494C4B00UL;

    
    
    
    
    
    
    private const ulong AMR_HEAD_LOWER = 0x2321414D52000000UL;

    
    
    
    
    
    
    private const ulong SILKV3_HEAD_LOWER = 0x232153494C4B0000UL;

    
    
    
    
    
    
    private const ulong OGG_HEAD_LOWER = 0x4F67675300000000UL;

    
    
    
    
    
    
    private const ulong MP3ID3_HEAD_LOWER = 0x4944330000000000UL;

    
    
    
    
    
    
    private const ulong MP3FFF2_HEAD_LOWER = 0xFFF2000000000000UL;

    
    
    
    
    
    
    private const ulong MP3FFF3_HEAD_LOWER = 0xFFF3000000000000UL;

    
    
    
    
    
    
    private const ulong MP3FFFB_HEAD_LOWER = 0xFFFB000000000000UL;

    
    
    
    
    public static AudioFormat DetectAudio(scoped ReadOnlySpan<byte> data)
    {
        ulong lower = BinaryPrimitives.ReadUInt64BigEndian(data.Slice(0, sizeof(ulong)));
        ulong upper = BinaryPrimitives.ReadUInt64BigEndian(data.Slice(8, sizeof(ulong)));

        if ((lower & WAV_HEAD_LOWER) == WAV_HEAD_LOWER && (upper & WAV_HEAD_UPPER) == WAV_HEAD_UPPER) return AudioFormat.Wav;

        if ((lower & TENSILKV3_HEAD_LOWER) == TENSILKV3_HEAD_LOWER) return AudioFormat.TenSilkV3;

        if ((lower & AMR_HEAD_LOWER) == AMR_HEAD_LOWER) return AudioFormat.Amr;

        if ((lower & SILKV3_HEAD_LOWER) == SILKV3_HEAD_LOWER) return AudioFormat.SilkV3;

        if ((lower & OGG_HEAD_LOWER) == OGG_HEAD_LOWER) return AudioFormat.Ogg;

        if ((lower & MP3ID3_HEAD_LOWER) == MP3ID3_HEAD_LOWER) return AudioFormat.Mp3;

        if ((lower & MP3FFF2_HEAD_LOWER) == MP3FFF2_HEAD_LOWER) return AudioFormat.Mp3;

        if ((lower & MP3FFF3_HEAD_LOWER) == MP3FFF3_HEAD_LOWER) return AudioFormat.Mp3;

        if ((lower & MP3FFFB_HEAD_LOWER) == MP3FFFB_HEAD_LOWER) return AudioFormat.Mp3;

        return AudioFormat.Unknown;
    }

    private const int SilkHeaderLength = 9;
    
    private const ushort SilkEnd = 0xFFFF;

    public static float GetSilkTime(byte[] data, int offset = 0)
    {
        ref byte dataRef = ref data[0];
        
        int count = 0;
        for (int i = SilkHeaderLength + offset; 
             i < data.Length && Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref dataRef, i)) != SilkEnd;
             i += 2 + BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(i, 2)))
        {
            count++;
        }
        return count * 0.02f;
    }
    
    public static float GetTenSilkTime(byte[] data)
    {
        int count = 0;
        for (int i = SilkHeaderLength + 1; 
             i < data.Length; 
             i += 2 + BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(i, 2)))
        {
            count++;
        }
        return count * 0.02f;
    }
}
