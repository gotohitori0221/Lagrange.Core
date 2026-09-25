using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Notify;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class GroupRecallNudge
{
    [ProtoMember(1)] public string OperatorUid { get; set; }

    [ProtoMember(3)] public long GroupUin { get; set; }

    [ProtoMember(4)] public ulong BusiId { get; set; }

    [ProtoMember(5)] public ulong TipsSeqId { get; set; }
}
