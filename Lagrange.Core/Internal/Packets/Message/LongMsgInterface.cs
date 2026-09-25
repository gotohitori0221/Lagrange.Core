using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Message;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class PbMultiMsgTransmit
{
    [ProtoMember(1)] public List<CommonMessage> Messages { get; set; }
    
    [ProtoMember(2)] public List<PbMultiMsgItem> Items { get; set; }
}

[ProtoPackable]
internal partial class PbMultiMsgItem
{
    [ProtoMember(1)] public string FileName { get; set; }
    
    [ProtoMember(2)] public PbMultiMsgNew Buffer { get; set; }
}

[ProtoPackable]
internal partial class PbMultiMsgNew
{
    [ProtoMember(1)] public List<CommonMessage> Msg { get; set; }
}

[ProtoPackable]
internal partial class LongMsgInterfaceReq
{
    [ProtoMember(1)] public LongMsgRecvReq RecvReq { get; set; }
    
    [ProtoMember(2)] public LongMsgSendReq SendReq { get; set; }
    
    [ProtoMember(15)] public LongMsgAttr Attr { get; set; }
}

[ProtoPackable]
internal partial class LongMsgInterfaceRsp
{
    [ProtoMember(1)] public LongMsgRecvRsp RecvRsp { get; set; }
    
    [ProtoMember(2)] public LongMsgSendRsp SendRsp { get; set; }
    
    [ProtoMember(15)] public LongMsgAttr Attr { get; set; }
}

[ProtoPackable]
internal partial class LongMsgAttr
{
    [ProtoMember(1)] public uint SubCmd { get; set; } 
    
    [ProtoMember(2)] public uint ClientType { get; set; } 
    
    [ProtoMember(3)] public uint Platform { get; set; } 
    
    [ProtoMember(4)] public uint ProxyType { get; set; } 
}

[ProtoPackable]
internal partial class LongMsgPeerInfo
{
    [ProtoMember(2)] public string? PeerUid { get; set; }
}

[ProtoPackable]
internal partial class LongMsgRecvReq
{
    [ProtoMember(1)] public LongMsgPeerInfo? PeerInfo { get; set; }

    [ProtoMember(2)] public string ResId { get; set; }

    [ProtoMember(3)] public uint MsgType { get; set; }
}

[ProtoPackable]
internal partial class LongMsgSendReq
{
    [ProtoMember(1)] public uint MsgType { get; set; }
    
    [ProtoMember(2)] public LongMsgPeerInfo? PeerInfo { get; set; }
    
    [ProtoMember(3)] public long GroupUin { get; set; }
    
    [ProtoMember(4)] public byte[] Payload { get; set; }
}

[ProtoPackable]
internal partial class LongMsgSendRsp
{
    [ProtoMember(3)] public string ResId { get; set; }
}

[ProtoPackable]
internal partial class LongMsgRecvRsp
{
    [ProtoMember(3)] public string ResId { get; set; }
    
    [ProtoMember(4)] public byte[] Payload { get; set; }
}