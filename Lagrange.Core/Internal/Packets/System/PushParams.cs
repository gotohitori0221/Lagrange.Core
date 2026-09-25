using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.System;

[ProtoPackable]
internal partial class PushParams
{
    [ProtoMember(4)] public List<OnlineDevice> OnlineDevices { get; set; } = new();

    [ProtoMember(6)] public GuildParams GuildParams { get; set; } = new();

    [ProtoMember(7)] public string ErrMsg { get; set; } = string.Empty;

    [ProtoMember(9)] public uint GroupMsgStorageTime { get; set; }
}

[ProtoPackable]
internal partial class GuildParams
{
    [ProtoMember(1)] public uint GuildFlag { get; set; }

    [ProtoMember(2)] public uint GuildSwitchFlag { get; set; }
}

[ProtoPackable]
internal partial class OnlineDevice
{
    [ProtoMember(1)] public uint InstId { get; set; }

    [ProtoMember(2)] public uint ClientType { get; set; }

    [ProtoMember(3)] public uint State { get; set; }

    [ProtoMember(4)] public uint PlatId { get; set; }

    [ProtoMember(5)] public string PlatType { get; set; } = string.Empty;

    [ProtoMember(6)] public uint NewClientType { get; set; }

    [ProtoMember(7)] public string DeviceName { get; set; } = string.Empty;
}