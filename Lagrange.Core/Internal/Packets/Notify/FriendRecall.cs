#pragma warning disable CS8618

using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Notify;

[ProtoPackable]
internal partial class FriendRecall
{
    [ProtoMember(1)] public FriendRecallInfo Info { get; set; }
}

[ProtoPackable]
internal partial class FriendRecallInfo
{
    [ProtoMember(1)] public string FromUid { get; set; }

    [ProtoMember(2)] public string ToUid { get; set; }

    [ProtoMember(13)] public FriendRecallTipInfo TipInfo { get; set; }

    [ProtoMember(20)] public long Sequence { get; set; }
}

[ProtoPackable]
internal partial class FriendRecallTipInfo
{
    [ProtoMember(2)] public string? Tip { get; set; }
}