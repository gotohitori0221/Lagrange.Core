using Lagrange.Proto;
using Lagrange.Proto.Serialization;

namespace Lagrange.Core.Internal.Packets.Service;

[ProtoPackable]
internal partial class C501ReqBody
{
    [ProtoMember(1281)] public SubCmd0x501ReqBody ReqBody { get; set; } = new();
}

[ProtoPackable]
internal partial class C501RspBody
{
    [ProtoMember(1281)] public SubCmd0x501RspBody RspBody { get; set; } = new();
}

[ProtoPackable]
internal partial class SubCmd0x501ReqBody
{
    [ProtoMember(1)] public ulong Uin { get; set; }
    
    [ProtoMember(2)] public uint IdcId { get; set; }
    
    [ProtoMember(3)] public uint Appid { get; set; }
    
    [ProtoMember(4)] public uint LoginSigType { get; set; }
    
    [ProtoMember(5)] public byte[] LoginSigTicket { get; set; } = [];
    
    [ProtoMember(6)] public uint RequestFlag { get; set; }
    
    [ProtoMember(7)] public uint[] ServiceTypes { get; set; } = [];
    
    [ProtoMember(8)] public uint Bid { get; set; }
    
    [ProtoMember(9)] public int Field9 { get; set; } 

    [ProtoMember(10)] public int Field10 { get; set; } 

    [ProtoMember(11)] public int Field11 { get; set; } 
    
    [ProtoMember(15)] public string Version { get; set; } = string.Empty;
}

[ProtoPackable]
internal partial class SubCmd0x501RspBody
{
    [ProtoMember(1)] public byte[] SigSession { get; set; } = [];
    
    [ProtoMember(2)] public byte[] SessionKey { get; set; } = [];
    
    [ProtoMember(3)] public List<SrvAddrs> Addrs { get; set; } = [];
}

[ProtoPackable]
internal partial class SrvAddrs
{
    [ProtoMember(1)] public uint ServiceType { get; set; }
    
    [ProtoMember(2)] public List<IpAddr> Addrs { get; set; } = [];
}

[ProtoPackable]
internal partial class IpAddr
{
    [ProtoMember(1)] public uint Type { get; set; }
    
    [ProtoMember(2, NumberHandling = ProtoNumberHandling.Fixed32)] public uint Ip { get; set; }
    
    [ProtoMember(3)] public uint Port { get; set; }
    
    [ProtoMember(4)] public uint Area { get; set; }
}