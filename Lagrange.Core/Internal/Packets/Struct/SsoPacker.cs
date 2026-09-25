using Lagrange.Core.Common;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Compression;
using Lagrange.Core.Utility.Extension;

namespace Lagrange.Core.Internal.Packets.Struct;

internal class SsoPacker(BotContext context) : StructBase(context)
{
    private const string Hex = "0123456789abcdef";

    private readonly Lazy<string> _guid = new(() => Convert.ToHexString(context.Keystore.Guid).ToLowerInvariant());
    private readonly BotContext _context = context;

    public BinaryPacket BuildProtocol12(BotSsoPacket sso, SsoSecureInfo? secInfo)
    {
        var head = new BinaryPacket(stackalloc byte[0x200]);
        
        head.Write(sso.Sequence); 
        head.Write(AppInfo.SubAppId); 
        head.Write(2052); 
        head.Write([0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00]);
        head.Write(Keystore.WLoginSigs.A2, Prefix.Int32 | Prefix.WithPrefix); 
        head.Write(sso.Command, Prefix.Int32 | Prefix.WithPrefix); 
        head.Write(ReadOnlySpan<byte>.Empty, Prefix.Int32 | Prefix.WithPrefix); 
        head.Write(_guid.Value, Prefix.Int32 | Prefix.WithPrefix); 
        head.Write(ReadOnlySpan<byte>.Empty, Prefix.Int32 | Prefix.WithPrefix);
        head.Write(AppInfo.CurrentVersion, Prefix.Int16 | Prefix.WithPrefix);
        WriteSsoReservedField(ref head, secInfo);
        
        var headSpan = head.CreateReadOnlySpan();
        var result = new BinaryPacket(headSpan.Length + sso.Data.Length + 2 * 4); 
        
        result.Write(headSpan, Prefix.Int32 | Prefix.WithPrefix);
        result.Write(sso.Data.Span, Prefix.Int32 | Prefix.WithPrefix); 
        
        return result;
    }
    
    public BinaryPacket BuildProtocol13(BotSsoPacket sso, SsoSecureInfo? secInfo)
    {
        var head = new BinaryPacket(stackalloc byte[0x200]);
        
        head.Write(sso.Command, Prefix.Int32 | Prefix.WithPrefix); 
        head.Write(ReadOnlySpan<byte>.Empty, Prefix.Int32 | Prefix.WithPrefix); 
        WriteSsoReservedField(ref head, secInfo);
        
        var headSpan = head.CreateReadOnlySpan();
        var result = new BinaryPacket(headSpan.Length + sso.Data.Length + 2 * 4); 
        
        result.Write(headSpan, Prefix.Int32 | Prefix.WithPrefix);
        result.Write(sso.Data.Span, Prefix.Int32 | Prefix.WithPrefix); 
        
        return result;
    }

    public BotSsoPacket Parse(ReadOnlySpan<byte> data)
    {
        var parent = new BinaryPacket(data);
        var head = parent.ReadBytes(Prefix.Int32 | Prefix.WithPrefix);
        var body = parent.ReadBytes(Prefix.Int32 | Prefix.WithPrefix);
        
        var headReader = new BinaryPacket(head);
        int sequence = headReader.Read<int>();
        int retCode = headReader.Read<int>();
        string extra = headReader.ReadString(Prefix.Int32 | Prefix.WithPrefix);
        string command = headReader.ReadString(Prefix.Int32 | Prefix.WithPrefix);
        var msgCookie = headReader.ReadBytes(Prefix.Int32 | Prefix.WithPrefix);
        int dataFlag = headReader.Read<int>();
        var reserveField = headReader.ReadBytes(Prefix.Int32 | Prefix.WithPrefix);
        
        ReadOnlyMemory<byte> payload = dataFlag switch
        {
            0 or 4 => body.ToArray(), 
            1 => ZCompression.ZDecompress(body, false),
            _ => throw new ArgumentOutOfRangeException(nameof(dataFlag))
        };

        return retCode == 0
            ? new BotSsoPacket(command, payload, sequence)
            : new BotSsoPacket(command, sequence, retCode, extra);
    }

    private void WriteSsoReservedField(ref BinaryPacket writer, SsoSecureInfo? secInfo)
    {
        Span<char> trace = stackalloc char[55];

        trace[0] = '0';
        trace[1] = '1';
        trace[2] = '-';
        for (int i = 3; i < 35; i++) trace[i] = Hex[Random.Shared.Next(0, Hex.Length)];
        trace[35] = '-';
        for (int i = 36; i < 52; i++) trace[i] = Hex[Random.Shared.Next(0, Hex.Length)];
        trace[52] = '-';
        trace[53] = '0';
        trace[54] = '1';
        
        var reserved = new SsoReserveFields
        {
            TraceParent = new string(trace),
            Uid = Keystore.Uid,
            SecInfo = secInfo
        };

        if (_context.Config.Protocol.IsAndroid())
        {
            reserved.MsgType = 32;
            reserved.NtCoreVersion = 100;
        }
     
        writer.EnterLengthBarrier<int>();
        ProtoHelper.Serialize(ref writer, reserved);
        writer.ExitLengthBarrier<int>(true);
    }
}