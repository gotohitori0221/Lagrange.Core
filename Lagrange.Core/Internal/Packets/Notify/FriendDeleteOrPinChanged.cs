using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Notify;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class FriendDeleteOrPinChanged
{
    [ProtoMember(1)] public FriendDeleteOrPinChangedBody Body { get; set; }
}

[ProtoPackable]
internal partial class FriendDeleteOrPinChangedBody
{
    
    [ProtoMember(2)] public uint Type { get; set; }

    [ProtoMember(20)] public PinChanged? PinChanged { get; set; }
}

[ProtoPackable]
internal partial class PinChanged
{
    [ProtoMember(1)] public PinChangedBody Body { get; set; }
}

[ProtoPackable]
internal partial class PinChangedBody
{
    [ProtoMember(1)] public string Uid { get; set; }

    [ProtoMember(2)] public uint? GroupUin { get; set; }

    [ProtoMember(400)] public PinChangedInfo Info { get; set; }
}

[ProtoPackable]
internal partial class PinChangedInfo
{
    
    
    [ProtoMember(2)] public byte[] Timestamp { get; set; }
}
