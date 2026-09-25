using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618




[ProtoPackable]
internal partial class D7E5ReqBody
{
    [ProtoMember(11)] public string? TargetUid { get; set; }

    [ProtoMember(12)] public uint Field2 { get; set; } = 71;

    [ProtoMember(13)] public uint Count { get; set; } = 1;
}
