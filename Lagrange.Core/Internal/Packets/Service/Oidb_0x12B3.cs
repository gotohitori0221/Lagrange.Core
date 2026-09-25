using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618




[ProtoPackable]
internal partial class D12B3RspBody
{
    [ProtoMember(1)] public List<D12B3RspBodyFriend>? Friends { get; set; }

    [ProtoMember(2)] public List<D12B3RspBodyGroup>? Groups { get; set; }
}

[ProtoPackable]
internal partial class D12B3RspBodyFriend
{
    [ProtoMember(1)] public string Uid { get; set; }
}

[ProtoPackable]
internal partial class D12B3RspBodyGroup
{
    [ProtoMember(1)] public uint Uin { get; set; }
}
