using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Login;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class DevInfo
{
    [ProtoMember(1)] public string DevType { get; set; }
    
    [ProtoMember(2)] public string DevName { get; set; }
}

[ProtoPackable]
internal partial class GenInfo
{
    [ProtoMember(1)] public uint? ClientType { get; set; }
    
    [ProtoMember(2)] public uint? ClientVer { get; set; }
    
    [ProtoMember(3)] public uint? ClientAppid { get; set; }
    
    [ProtoMember(6)] public uint Field6 { get; set; }
}

[ProtoPackable]
internal partial class QrExtInfo
{
    [ProtoMember(1)] public DevInfo DevInfo { get; set; }
    
    [ProtoMember(2)] public string QrUrl { get; set; }
    
    [ProtoMember(3)] public string QrSig { get; set; }
    
    [ProtoMember(4)] public GenInfo GenInfo { get; set; }
}

[ProtoPackable]
internal partial class ScanExtInfo
{
    [ProtoMember(1)] public byte[] Guid { get; set; }
    
    [ProtoMember(2)] public string Imei { get; set; }
    
    [ProtoMember(3)] public uint ScanScene { get; set; }
    
    [ProtoMember(4)] public bool AllowAutoRenewTicket { get; set; }
    
    [ProtoMember(5)] public bool? InvalidGenTicket { get; set; }
}