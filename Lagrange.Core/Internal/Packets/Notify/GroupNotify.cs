using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Notify;

#pragma warning disable CS8618


[ProtoPackable]
internal partial class GroupAdminChangeBody
{
    [ProtoMember(1)] public ulong GroupUin { get; set; }

    [ProtoMember(2)] public string TargetUid { get; set; }

    [ProtoMember(3)] public uint IsAdmin { get; set; } 
}


[ProtoPackable]
internal partial class GroupNameChangeBody
{
    [ProtoMember(1)] public ulong GroupUin { get; set; }

    [ProtoMember(2)] public string NewName { get; set; }

    [ProtoMember(3)] public string OperatorUid { get; set; }

    [ProtoMember(4)] public string OldName { get; set; }
}


[ProtoPackable]
internal partial class GroupMuteBody
{
    [ProtoMember(1)] public uint GroupUin { get; set; }

    [ProtoMember(4)] public string? OperatorUid { get; set; }

    [ProtoMember(5)] public GroupMuteInfo? Info { get; set; }
}

[ProtoPackable]
internal partial class GroupMuteInfo
{
    [ProtoMember(3)] public GroupMuteState? State { get; set; }
}

[ProtoPackable]
internal partial class GroupMuteState
{
    [ProtoMember(1)] public string? TargetUid { get; set; }

    [ProtoMember(2)] public uint Duration { get; set; }
}
