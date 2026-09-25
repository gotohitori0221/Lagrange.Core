#pragma warning disable CS8618

using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Message;




[ProtoPackable]
internal partial class SsoGroupRecallMsgReq
{
    [ProtoMember(1)] public uint Type { get; set; } 

    [ProtoMember(2)] public long GroupUin { get; set; }

    [ProtoMember(3)] public SsoGroupRecallMsgReqField3 Field3 { get; set; }

    [ProtoMember(4)] public SsoGroupRecallMsgReqField4 Field4 { get; set; }
}

[ProtoPackable]
internal partial class SsoGroupRecallMsgReqField3
{
    [ProtoMember(1)] public ulong Sequence { get; set; }

    [ProtoMember(2)] public uint Random { get; set; }

    [ProtoMember(3)] public uint Field3 { get; set; } 
}

[ProtoPackable]
internal partial class SsoGroupRecallMsgReqField4
{
    [ProtoMember(1)] public uint Field1 { get; set; } 
}