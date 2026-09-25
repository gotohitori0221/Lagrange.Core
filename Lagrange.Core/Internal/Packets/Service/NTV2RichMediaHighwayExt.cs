using Lagrange.Proto;

#pragma warning disable CS8618

namespace Lagrange.Core.Internal.Packets.Service;

[ProtoPackable]
internal partial class NTV2RichMediaHighwayExt
{
    [ProtoMember(1)] public string FileUuid { get; set; }
    
    [ProtoMember(2)] public string UKey { get; set; }
    
    [ProtoMember(5)] public NTHighwayNetwork Network { get; set; }
    
    [ProtoMember(6)] public List<MsgInfoBody> MsgInfoBody { get; set; }
    
    [ProtoMember(10)] public uint BlockSize { get; set; }
    
    [ProtoMember(11)] public NTHighwayHash Hash { get; set; }
}

[ProtoPackable]
internal partial class NTHighwayHash
{
    [ProtoMember(1)] public List<byte[]> FileSha1 { get; set; }
}

[ProtoPackable]
internal partial class NTHighwayNetwork
{
    [ProtoMember(1)] public List<NTHighwayIPv4> IPv4s { get; set; }
}


[ProtoPackable]
internal partial class NTHighwayIPv4
{
    [ProtoMember(1)] public NTHighwayDomain Domain { get; set; }
    
    [ProtoMember(2)] public uint Port { get; set; }
}

[ProtoPackable]
internal partial class NTHighwayDomain
{
    [ProtoMember(1)] public bool IsEnable { get; set; }  
    
    [ProtoMember(2)] public string IP { get; set; }
}