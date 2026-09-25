using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class D102AReqBody
{
    [ProtoMember(1)] public List<string> Domain { get; set; }
}

[ProtoPackable]
internal partial class D102ARspBody
{
    [ProtoMember(1)] public Dictionary<string, string> PsKeys { get; set; }
    
    [ProtoMember(2)] public int KeyType { get; set; }
    
    [ProtoMember(3)] public string ClientKey { get; set; }
    
    [ProtoMember(4)] public uint Expiration { get; set; }
}