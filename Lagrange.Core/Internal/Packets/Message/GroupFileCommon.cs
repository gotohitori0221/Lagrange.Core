using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Message;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class FeedsInfo
{
    [ProtoMember(1)] public uint BusId { get; set; }
    
    [ProtoMember(2)] public string FileId { get; set; }
    
    [ProtoMember(3)] public uint MsgRandom { get; set; }
    
    [ProtoMember(4)] public byte[] Ext { get; set; }
    
    [ProtoMember(5)] public uint FeedFlag { get; set; }
}

[ProtoPackable]
internal partial class FeedsResult
{
    [ProtoMember(1)] public int RetCode { get; set; }
    
    [ProtoMember(2)] public string Detail { get; set; }
    
    [ProtoMember(3)] public string FileId { get; set; }
    
    [ProtoMember(4)] public uint BusId { get; set; }
    
    [ProtoMember(5)] public uint DeadTime { get; set; }
}

[ProtoPackable]
internal partial class FileInfo
{
    [ProtoMember(1)] public string FileId { get; set; }
    
    [ProtoMember(2)] public string FileName { get; set; }
    
    [ProtoMember(3)] public ulong FileSize { get; set; }
    
    [ProtoMember(4)] public uint BusId { get; set; }
    
    [ProtoMember(5)] public ulong UploadedSize { get; set; }
    
    [ProtoMember(6)] public uint UploadTime { get; set; }
    
    [ProtoMember(7)] public uint DeadTime { get; set; }
    
    [ProtoMember(8)] public uint ModifyTime { get; set; }
    
    [ProtoMember(9)] public uint DownloadTimes { get; set; }
    
    [ProtoMember(10)] public byte[] Sha { get; set; }
    
    [ProtoMember(11)] public byte[] Sha3 { get; set; }
    
    [ProtoMember(12)] public byte[] Md5 { get; set; }
    
    [ProtoMember(13)] public string LocalPath { get; set; }
    
    [ProtoMember(14)] public string UploaderName { get; set; }
    
    [ProtoMember(15)] public ulong UploaderUin { get; set; }
    
    [ProtoMember(16)] public string ParentFolderId { get; set; }
    
    [ProtoMember(17)] public uint SafeType { get; set; }
    
    [ProtoMember(20)] public byte[] FileBlobExt { get; set; }
    
    [ProtoMember(21)] public ulong OwnerUin { get; set; }
    
    [ProtoMember(22)] public string FeedId { get; set; }
    
    [ProtoMember(23)] public byte[] ReservedField { get; set; }
}

[ProtoPackable]
internal partial class FileInfoTmem
{
    [ProtoMember(1)] public ulong GroupCode { get; set; }
    
    [ProtoMember(2)] public List<FileInfo> Files { get; set; }
}

[ProtoPackable]
internal partial class FileItem
{
    [ProtoMember(1)] public uint Type { get; set; }
    
    
    [ProtoMember(2)] public FolderInfo FolderInfo { get; set; }
    
    [ProtoMember(3)] public FileInfo FileInfo { get; set; }
}

[ProtoPackable]
internal partial class FolderInfo
{
    [ProtoMember(1)]  public string FolderId { get; set; }
    
    [ProtoMember(2)]  public string ParentFolderId { get; set; }
    
    [ProtoMember(3)]  public string FolderName { get; set; }
    
    [ProtoMember(4)]  public uint CreateTime { get; set; }
    
    [ProtoMember(5)]  public uint ModifyTime { get; set; }
    
    [ProtoMember(6)]  public ulong CreateUin { get; set; }
    
    [ProtoMember(7)]  public string CreatorName { get; set; }
    
    [ProtoMember(8)]  public uint TotalFileCount { get; set; }
    
    [ProtoMember(9)]  public ulong ModifyUin { get; set; }
    
    [ProtoMember(10)] public string ModifyName { get; set; }
    
    [ProtoMember(11)] public ulong UsedSpace { get; set; }
}

[ProtoPackable]
internal partial class FolderInfoItem
{
    [ProtoMember(1)] public ulong GroupCode { get; set; }
    
    [ProtoMember(2)] public List<FolderInfo> Folders { get; set; }
}

[ProtoPackable]
internal partial class OverwriteInfo
{
    [ProtoMember(1)] public string FileId { get; set; }
    
    [ProtoMember(2)] public uint DownloadTimes { get; set; }
}