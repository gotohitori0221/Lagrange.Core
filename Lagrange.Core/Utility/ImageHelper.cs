using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Lagrange.Core.Utility;

internal static class ImageHelper
{
    public static ImageFormat Resolve(scoped Span<byte> span, out Vector2 size)
    {
        if (BinaryPrimitives.ReadUInt64BigEndian(span) == 0x0000000C6A502020) 
        {
            size = new Vector2(BinaryPrimitives.ReadUInt32BigEndian(span[0x30..0x34]), BinaryPrimitives.ReadUInt32BigEndian(span[0x34..0x38]));
            return ImageFormat.Jpeg2000;
        }
        
        if (span[..6].SequenceEqual("GIF89a"u8) || span[..6].SequenceEqual("GIF87a"u8)) 
        {
            size = new Vector2(BitConverter.ToUInt16(span[6..8]), BitConverter.ToUInt16(span[8..10]));
            return ImageFormat.Gif;
        }

        if (span[..2].SequenceEqual(new byte[] { 0xFF, 0xD8 })) 
        {
            size = Vector2.Zero;
            
            for (int i = 2; i < span.Length - 10; i++)
            {
                if ((Unsafe.ReadUnaligned<ushort>(ref span[i]) & 0xFCFF) == 0xC0FF) 
                {
                    size = new Vector2(BinaryPrimitives.ReadUInt16BigEndian(span[(i + 7)..(i + 9)]), BinaryPrimitives.ReadUInt16BigEndian(span[(i + 5)..(i + 7)]));
                    break;
                }
            }
            return ImageFormat.Jpeg;
        }

        if (span[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A })) 
        {
            size = new Vector2(BinaryPrimitives.ReadUInt32BigEndian(span[16..20]), BinaryPrimitives.ReadUInt32BigEndian(span[20..24]));
            return ImageFormat.Png;
        }

        if (span[..4].SequenceEqual("RIFF"u8) && span[8..12].SequenceEqual("WEBP"u8)) 
        {
            if (span[12..16].SequenceEqual("VP8X"u8)) 
            {
                size = new Vector2(BitConverter.ToUInt16(span[24..27]) + 1, BitConverter.ToUInt16(span[27..30]) + 1);
            }
            else if (span[12..16].SequenceEqual("VP8L"u8)) 
            {
                size = new Vector2((BitConverter.ToInt32(span[21..25]) & 0x3FFF) + 1, ((BitConverter.ToInt32(span[21..25]) & 0xFFFC000) >> 0x0E) + 1);
            }
            else 
            {
                size = new Vector2(BitConverter.ToUInt16(span[26..28]), BitConverter.ToUInt16(span[28..30]));
            }
            
            return ImageFormat.Webp;
        }

        if (span[..2].SequenceEqual("BM"u8)) 
        {
            size = new Vector2(BitConverter.ToUInt16(span[18..20]), BitConverter.ToUInt16(span[22..24]));
            return ImageFormat.Bmp;
        }

        if (span[..2].SequenceEqual("II"u8) || span[..2].SequenceEqual("MM"u8)) 
        {
            size = new Vector2(BitConverter.ToUInt16(span[18..20]), BitConverter.ToUInt16(span[30..32]));
            return ImageFormat.Tiff;
        }

        size = Vector2.Zero;
        return ImageFormat.Unknown;
    }
}

internal enum ImageFormat : uint
{
    Unknown,  
    Png = 1001,
    Jpeg = 1000,
    Jpeg2000 = 1003,
    Gif = 2000,
    Webp = 1002,
    Bmp = 1005,
    Tiff
}