using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

[ProtoPackable]
internal partial class DED3ReqBody
{
    [ProtoMember(1)] public long ToUin { get; set; } 
    
    [ProtoMember(2)] public long GroupCode { get; set; } 
    
    [ProtoMember(3)] public uint MsgSeq { get; set; } 
    
    [ProtoMember(4)] public uint MsgRand { get; set; } 
    
    [ProtoMember(5)] public long AioUin { get; set; } 
    
    [ProtoMember(6)] public uint NudgeType { get; set; } 
}

[ProtoPackable]
internal partial class DED3RspBody;