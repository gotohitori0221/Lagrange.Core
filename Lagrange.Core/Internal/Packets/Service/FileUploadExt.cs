using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class FileUploadExt
{
    [ProtoMember(1)] public int Unknown1 { get; set; }
    
    [ProtoMember(2)] public int Unknown2 { get; set; }
    
    [ProtoMember(3)] public int Unknown3 { get; set; }
    
    [ProtoMember(100)] public FileUploadEntry Entry { get; set; }
    
    [ProtoMember(200)] public int Unknown200 { get; set; }
}

[ProtoPackable]
internal partial class FileUploadEntry
{
    [ProtoMember(100)] public ExcitingBusiInfo BusiBuff { get; set; }
    
    [ProtoMember(200)] public ExcitingFileEntry FileEntry { get; set; }
    
    [ProtoMember(300)] public ExcitingClientInfo ClientInfo { get; set; }
    
    [ProtoMember(400)] public ExcitingFileNameInfo FileNameInfo { get; set; }
    
    [ProtoMember(500)] public ExcitingHostConfig Host { get; set; }
}

[ProtoPackable]
internal partial class ExcitingBusiInfo
{
    [ProtoMember(1)] public int BusId { get; set; }
    
    [ProtoMember(100)] public long SenderUin { get; set; }
    
    [ProtoMember(200)] public long ReceiverUin { get; set; }
    
    [ProtoMember(400)] public long GroupCode { get; set; }
}

[ProtoPackable]
internal partial class ExcitingFileEntry
{
    [ProtoMember(100)] public long FileSize { get; set; }
    
    [ProtoMember(200)] public byte[] Md5 { get; set; }
    
    [ProtoMember(300)] public byte[] CheckKey { get; set; }
    
    [ProtoMember(400)] public byte[] Md510M { get; set; }
    
    [ProtoMember(500)] public byte[] Sha3 { get; set; }
    
    [ProtoMember(600)] public string FileId { get; set; }
    
    [ProtoMember(700)] public byte[] UploadKey { get; set; }
}

[ProtoPackable]
internal partial class ExcitingClientInfo
{
    [ProtoMember(100)] public int ClientType { get; set; }
    
    [ProtoMember(200)] public string AppId { get; set; }
    
    [ProtoMember(300)] public int TerminalType { get; set; }
    
    [ProtoMember(400)] public string ClientVer { get; set; }
    
    [ProtoMember(600)] public int Unknown { get; set; }
}

[ProtoPackable]
internal partial class ExcitingFileNameInfo
{
    [ProtoMember(100)] public string FileName { get; set; }
}

[ProtoPackable]
internal partial class ExcitingHostConfig
{
    [ProtoMember(200)] public List<ExcitingHostInfo> Hosts { get; set; }
}

[ProtoPackable]
internal partial class ExcitingHostInfo
{
    [ProtoMember(1)] public ExcitingUrlInfo Url { get; set; }
    
    [ProtoMember(2)] public uint Port { get; set; }
}

[ProtoPackable]
internal partial class ExcitingUrlInfo
{
    [ProtoMember(1)] public int Unknown { get; set; }
    
    [ProtoMember(2)] public string Host { get; set; }
}