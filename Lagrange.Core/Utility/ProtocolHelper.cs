using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Utility;

internal static class ProtocolHelper
{
    public static Dictionary<ushort, byte[]> TlvUnPack(ref BinaryPacket reader)
    {
        ushort count = reader.Read<ushort>();
        var tlv = new Dictionary<ushort, byte[]>(count);

        for (int i = 0; i < count; i++)
        {
            ushort tag = reader.Read<ushort>();
            ushort length = reader.Read<ushort>();
            
            var data = new byte[length];
            reader.ReadBytes(data.AsSpan());
            tlv[tag] = data;
        }
        
        return tlv;
    }
    
    public static string UInt32ToIPV4Addr(uint i)
    {
        Span<byte> ip = stackalloc byte[4];
        
        ip[0] = (byte)(i & 0xFF);
        ip[1] = (byte)((i >> 8) & 0xFF);
        ip[2] = (byte)((i >> 16) & 0xFF);
        ip[3] = (byte)((i >> 24) & 0xFF);
        
        return $"{ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}";
    }
}