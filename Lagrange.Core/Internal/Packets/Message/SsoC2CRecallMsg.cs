#pragma warning disable CS8618

using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Message;

[ProtoPackable]
internal partial class SsoC2CRecallMsgReq
{
    [ProtoMember(1)] public uint Type { get; set; } 
    
    [ProtoMember(3)] public string TargetUid { get; set; } 
    
    [ProtoMember(4)] public SsoC2CRecallMsgReqInfo Info { get; set; }
    
    [ProtoMember(5)] public SsoC2CRecallMsgReqSettings Settings { get; set; }
    
    [ProtoMember(6)] public bool Field6 { get; set; } 
}

[ProtoPackable]
internal partial class SsoC2CRecallMsgReqInfo
{
    [ProtoMember(1)] public ulong Sequence { get; set; }
    
    [ProtoMember(2)] public uint Random { get; set; }
    
    [ProtoMember(3)] public ulong MessageId { get; set; } 
    
    [ProtoMember(4)] public uint Timestamp { get; set; }
    
    [ProtoMember(5)] public uint Field5 { get; set; } 
    
    [ProtoMember(6)] public ulong ClientSequence { get; set; } 
}

[ProtoPackable]
internal partial class SsoC2CRecallMsgReqSettings
{
    [ProtoMember(1)] public bool Field1 { get; set; } 
    
    [ProtoMember(2)] public bool Field2 { get; set; } 
}