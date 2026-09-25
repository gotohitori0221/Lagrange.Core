using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.System;

[ProtoPackable]
internal partial class ThirdPartyLoginResponse
{
    [ProtoMember(1)] public ulong Seq { get; set; }
    
    [ProtoMember(9)] public RespCommonInfo CommonInfo { get; set; } = new();
}

[ProtoPackable]
internal partial class RespCommonInfo
{
    [ProtoMember(10)] public uint NeedVerifyScenes { get; set; }
    
    [ProtoMember(11)] public RspNT RspNT { get; set; } = new();
    
    [ProtoMember(12)] public uint A1Seq { get; set; }
}

[ProtoPackable]
internal partial class RspNT
{
    [ProtoMember(1)] public string Uid { get; set; } = "";
    
    [ProtoMember(2)] public byte[] Ua2 { get; set; } = [];
}