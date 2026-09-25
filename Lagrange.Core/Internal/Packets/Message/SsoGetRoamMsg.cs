using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Message;




[ProtoPackable]
internal partial class SsoGetRoamMsgReq
{
    [ProtoMember(1)] public string? PeerUid { get; set; }
    
    [ProtoMember(2)] public uint Time { get; set; }
    
    [ProtoMember(3)] public uint Random { get; set; }  
    
    [ProtoMember(4)] public uint Count { get; set; } 
    
    [ProtoMember(5)] public uint Direction { get; set; } 
}

[ProtoPackable]
internal partial class SsoGetRoamMsgRsp
{
    [ProtoMember(3)] public string? PeerUid { get; set; }
    
    [ProtoMember(4)] public bool IsComplete { get; set; }
    
    [ProtoMember(5)] public uint Timestamp { get; set; }

    [ProtoMember(6)] public uint Random { get; set; }

    [ProtoMember(7)] public List<CommonMessage> Messages { get; set; } = [];
}