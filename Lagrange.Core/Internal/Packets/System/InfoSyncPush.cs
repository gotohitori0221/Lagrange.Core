using Lagrange.Core.Internal.Packets.Message;
using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.System;

[ProtoPackable]
internal partial class InfoSyncPush
{
    [ProtoMember(1)] public uint Result { get; set; }
    
    [ProtoMember(2)] public string? ErrMsg { get; set; }
    
    [ProtoMember(3)] public uint PushFlag { get; set; }
    
    [ProtoMember(4)] public uint PushSeq { get; set; }
    
    [ProtoMember(5)] public uint RetryFlag { get; set; }

    [ProtoMember(6)] public List<GroupNode> GroupNodes { get; set; } = [];

    [ProtoMember(7)] public GroupSystemNotifications Notifications { get; set; } = new();
    
    [ProtoMember(8)] public SystemNotifications SysNotifications { get; set; } = new();
    
    [ProtoMember(10)] public uint UseInitCacheData { get; set; }

    [ProtoMember(11)] public GuildNode GuildNodes { get; set; } = new();
    
    [ProtoMember(12)] public uint DiscussListFlag { get; set; }
    
    [ProtoMember(13)] public uint RoamMsgOptimizeFlag { get; set; }
    
    [ProtoMember(14)] public uint GroupGuildFlag { get; set; }
}

[ProtoPackable]
internal partial class GroupNode
{
    [ProtoMember(1)] public ulong GroupCode { get; set; }
    
    [ProtoMember(2)] public ulong GroupSeq { get; set; }
    
    [ProtoMember(3)] public ulong ReadMsgSeq { get; set; }
    
    [ProtoMember(4)] public ulong Mask { get; set; }
    
    [ProtoMember(5)] public ulong LongestMsgTime { get; set; }
    
    [ProtoMember(6)] public bool HasMessage { get; set; }
    
    [ProtoMember(8)] public ulong LatestMsgTime { get; set; }
    
    [ProtoMember(9)] public string PeerName { get; set; } = string.Empty;
    
    [ProtoMember(10)] public ulong LongestMsgSeq { get; set; }
    
    [ProtoMember(11)] public ulong UinFlagEx2 { get; set; }
    
    [ProtoMember(12)] public uint ImportantMsgLatestSeq { get; set; }
    
    [ProtoMember(13)] public uint GroupMaxEventSeq { get; set; }
    
    [ProtoMember(14)] public uint Random { get; set; }
    
    [ProtoMember(15)] public uint NeedToCheckSeqOnAioOpen { get; set; }
}


[ProtoPackable]
internal partial class GuildNode
{
    [ProtoMember(1)] public ulong PeerId { get; set; }
}

[ProtoPackable]
internal partial class GroupSystemNotifications
{
    [ProtoMember(3)] public List<GroupSystemNotificationsInfo> Infos { get; set; } = [];
}

[ProtoPackable]
internal partial class SystemNotifications
{
    
    
    [ProtoMember(4)] public List<SystemNotificationsInfo> Infos { get; set; } = [];
}

[ProtoPackable]
internal partial class GroupSystemNotificationsInfo
{
    [ProtoMember(3)] public long GroupCode { get; set; }
    
    [ProtoMember(4)] public uint StartSeq { get; set; }
    
    [ProtoMember(5)] public uint EndSeq { get; set; }

    [ProtoMember(6)] public List<CommonMessage> Messages { get; set; } = [];
    
    [ProtoMember(8)] public long LastSpeakTimestamp { get; set; }
}

[ProtoPackable]
internal partial class SystemNotificationsInfo
{
    [ProtoMember(1)] public long PeerUin { get; set; }

    [ProtoMember(2)] public string PeerUid { get; set; } = string.Empty; 
    
    [ProtoMember(5)] public long LastSpeakTimestamp { get; set; }
    
    [ProtoMember(8)] public List<CommonMessage> Messages { get; set; } = [];
}