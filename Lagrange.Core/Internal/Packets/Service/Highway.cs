using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class ReqDataHighwayHead
{
    [ProtoMember(1)] public DataHighwayHead MsgBaseHead { get; set; }
    
    [ProtoMember(2)] public SegHead MsgSegHead { get; set; }
    
    [ProtoMember(3)] public ReadOnlyMemory<byte> BytesReqExtendInfo { get; set; }
    
    [ProtoMember(4)] public ulong Timestamp { get; set; }
    
    [ProtoMember(5)] public LoginSigHead MsgLoginSigHead { get; set; }
}

[ProtoPackable]
internal partial class RespDataHighwayHead
{
    [ProtoMember(1)] public DataHighwayHead MsgBaseHead { get; set; }
    
    [ProtoMember(2)] public SegHead? MsgSegHead { get; set; }
    
    [ProtoMember(3)] public uint ErrorCode { get; set; }
    
    [ProtoMember(4)] public uint AllowRetry { get; set; }
    
    [ProtoMember(5)] public uint CacheCost { get; set; }
    
    [ProtoMember(6)] public uint HtCost { get; set; }
    
    [ProtoMember(7)] public byte[] BytesRspExtendInfo { get; set; }
    
    [ProtoMember(8)] public ulong Timestamp { get; set; }
    
    [ProtoMember(9)] public ulong Range { get; set; }
    
    [ProtoMember(10)] public uint IsReset { get; set; }
}

[ProtoPackable]
internal partial class DataHighwayHead
{
    [ProtoMember(1)] public uint Version { get; set; }
    
    [ProtoMember(2)] public string? Uin { get; set; }
    
    [ProtoMember(3)] public string? Command { get; set; }
    
    [ProtoMember(4)] public uint Seq { get; set; }
    
    [ProtoMember(5)] public uint RetryTimes { get; set; }
    
    [ProtoMember(6)] public uint AppId { get; set; }
    
    [ProtoMember(7)] public uint DataFlag { get; set; }
    
    [ProtoMember(8)] public uint CommandId { get; set; }
    
    [ProtoMember(9)] public byte[]? BuildVer { get; set; }
    
    
    
    
}

[ProtoPackable]
internal partial class LoginSigHead
{
    [ProtoMember(1)] public uint Uint32LoginSigType { get; set; }
    
    [ProtoMember(2)] public byte[] BytesLoginSig { get; set; }
    
    [ProtoMember(3)] public uint AppId { get; set; }
}

[ProtoPackable]
internal partial class SegHead
{
    [ProtoMember(1)] public uint ServiceId { get; set; }
    
    [ProtoMember(2)] public ulong Filesize { get; set; }
    
    [ProtoMember(3)] public ulong DataOffset { get; set; }
    
    [ProtoMember(4)] public uint DataLength { get; set; }
    
    [ProtoMember(5)] public uint? RetCode { get; set; }

    [ProtoMember(6)] public byte[] ServiceTicket { get; set; } = [];
    
    
    
    [ProtoMember(8)] public byte[] Md5 { get; set; } = [];
    
    [ProtoMember(9)] public byte[] FileMd5 { get; set; } = [];
    
    [ProtoMember(10)] public uint CacheAddr { get; set; }
    
    
    
    
    
    [ProtoMember(13)] public uint CachePort { get; set; }
}