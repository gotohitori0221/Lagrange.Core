using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Security.Cryptography;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Utility.Cryptography;

internal static class PowProvider
{
    
    
    
    public static byte[] GenerateTlv547(ReadOnlySpan<byte> tlv546)
    {
        var reader = new BinaryPacket(tlv546);

        byte version = reader.Read<byte>();
        byte typ = reader.Read<byte>();
        byte hashType = reader.Read<byte>();
        bool ok = reader.Read<byte>() == 0;
        ushort maxIndex = reader.Read<ushort>();
        var reserved = new byte[2];
        reader.ReadBytes(reserved.AsSpan());
        
        var src = reader.ReadBytes(Prefix.Int16 | Prefix.LengthOnly);
        var tgt = reader.ReadBytes(Prefix.Int16 | Prefix.LengthOnly);
        var cpy = reader.ReadBytes(Prefix.Int16 | Prefix.LengthOnly);
        
        byte[] dst;
        int elapsed, cnt = 0;

        var inputNum = new BigInteger(src, true, true);
        if (tgt.Length == 32)
        {
            var start = DateTime.Now;
            var hash = SHA256.HashData(inputNum.ToByteArray(true, true));
            
            while (!Vector256.EqualsAll(Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetReference(tgt)), Unsafe.As<byte, Vector256<byte>>(ref MemoryMarshal.GetArrayDataReference(hash))))
            {
                inputNum++;
                hash = SHA256.HashData(inputNum.ToByteArray(true, true));
                cnt++;
                
                if (cnt > 6000000) throw new Exception("Calculating PoW cost too much time, maybe something wrong");
            }
            
            ok = true;
            dst = inputNum.ToByteArray(true, true);
            elapsed = (int)(DateTime.Now - start).TotalMilliseconds;
        }
        else
        {
            throw new InvalidOperationException("Only support SHA256 PoW");
        }
        
        var writer = new BinaryPacket(stackalloc byte[0x200]);
        
        writer.Write(version);
        writer.Write(typ);
        writer.Write(hashType);
        writer.Write(ok ? (byte)1 : (byte)0);
        writer.Write(maxIndex);
        writer.Write(reserved);
        writer.Write(src, Prefix.Int16 | Prefix.LengthOnly);
        writer.Write(tgt, Prefix.Int16 | Prefix.LengthOnly);
        writer.Write(cpy, Prefix.Int16 | Prefix.LengthOnly);
        writer.Write(dst, Prefix.Int16 | Prefix.LengthOnly);
        writer.Write(elapsed);
        writer.Write(cnt);
        
        return writer.ToArray();
    }

    
    
    
    public static byte[] GenerateTlv548()
    {
        var src = RandomNumberGenerator.GetBytes(128);
        src[0] = 21;
        
        var srcNum = new BigInteger(src, true, true);
        const int cnt = 10000;
        var dstNum = srcNum + new BigInteger(cnt);
        var dst = dstNum.ToByteArray(true, true);
        var tgt = SHA256.HashData(dst);
        
        var writer = new BinaryPacket(stackalloc byte[0x200]);
        
        writer.Write<byte>(1); 
        writer.Write<byte>(2); 
        writer.Write<byte>(1); 
        writer.Write<byte>(2); 
        writer.Write<ushort>(10); 
        writer.Write(new byte[2]); 
        writer.Write(src, Prefix.Int16 | Prefix.LengthOnly);
        writer.Write(tgt, Prefix.Int16 | Prefix.LengthOnly);

        var cpy = writer.CreateReadOnlySpan();
        writer.Write(cpy, Prefix.Int16 | Prefix.LengthOnly);

        var tlv546 = writer.CreateReadOnlySpan();
        return GenerateTlv547(tlv546);
    }
}