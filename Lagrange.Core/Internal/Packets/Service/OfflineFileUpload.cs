using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class OfflineFileUploadRequest
{
    [ProtoMember(1)] public uint Command { get; set; } 
    
    [ProtoMember(2)] public int Seq { get; set; } 
    
    [ProtoMember(19)] public ApplyUploadReqV3 Upload { get; set; }
    
    [ProtoMember(101)] public int BusinessId { get; set; } 
    
    [ProtoMember(102)] public int ClientType { get; set; } 
    
    [ProtoMember(200)] public int FlagSupportMediaPlatform { get; set; } 
}

[ProtoPackable]
internal partial class ApplyUploadReqV3
{
    [ProtoMember(10)] public string SenderUid { get; set; }
    
    [ProtoMember(20)] public string ReceiverUid { get; set; }
    
    [ProtoMember(30)] public uint FileSize { get; set; }
    
    [ProtoMember(40)] public string FileName { get; set; }
    
    [ProtoMember(50)] public byte[] Md510MCheckSum { get; set; }
    
    [ProtoMember(60)] public byte[] Sha1CheckSum { get; set; }
    
    [ProtoMember(70)] public string LocalPath { get; set; }
    
    [ProtoMember(110)] public byte[] Md5CheckSum { get; set; }
    
    [ProtoMember(120)] public byte[] Sha3CheckSum { get; set; }
}

[ProtoPackable]
internal partial class OfflineFileUploadResponse 
{
    [ProtoMember(1)] public uint Command { get; set; } 
    
    [ProtoMember(2)] public int Seq { get; set; } 
    
    [ProtoMember(19)] public ApplyUploadRespV3 Upload { get; set; }
    
    [ProtoMember(101)] public int BusinessId { get; set; } 
    
    [ProtoMember(102)] public int ClientType { get; set; } 
    
    [ProtoMember(200)] public int FlagSupportMediaPlatform { get; set; } 
}

[ProtoPackable]
internal partial class ApplyUploadRespV3
{
    [ProtoMember(10)] public int RetCode { get; set; }
    
    [ProtoMember(20)] public string RetMsg { get; set; }
    
    [ProtoMember(30)] public long TotalSpace { get; set; }
    
    [ProtoMember(40)] public long UsedSpace { get; set; }
    
    [ProtoMember(50)] public long UploadedSize { get; set; }
    
    [ProtoMember(60)] public string UploadIp { get; set; }
    
    [ProtoMember(70)] public string UploadDomain { get; set; }
    
    [ProtoMember(80)] public uint UploadPort { get; set; }
    
    [ProtoMember(90)] public string Uuid { get; set; }
    
    [ProtoMember(100)] public byte[] UploadKey { get; set; }
    
    [ProtoMember(110)] public bool BoolFileExist { get; set; }
    
    [ProtoMember(120)] public int PackSize { get; set; }
    
    [ProtoMember(130)] public List<string> UploadIpList { get; set; }
    
    [ProtoMember(140)] public int UploadHttpsPort { get; set; }
    
    [ProtoMember(150)] public string UploadHttpsDomain { get; set; }
    
    [ProtoMember(160)] public string UploadDns { get; set; }
    
    [ProtoMember(170)] public string UploadLanip { get; set; }
    
    [ProtoMember(200)] public string FileIdCrc { get; set; }
    
    [ProtoMember(210)] public List<Addr> RtpMediaPlatformUploadAddress { get; set; }
    
    [ProtoMember(220)] public byte[] MediaPlatformUploadKey { get; set; }
}

[ProtoPackable]
internal partial class Addr
{
    [ProtoMember(1)] public uint OutIp { get; set; }
    
    [ProtoMember(2)] public uint OutPort { get; set; }
    
    [ProtoMember(3)] public uint InnerIp { get; set; }
    
    [ProtoMember(4)] public uint InnerPort { get; set; }
    
    [ProtoMember(5)] public uint IpType { get; set; }
}