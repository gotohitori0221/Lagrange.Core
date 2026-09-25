using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class DeleteFileReqBody
{
    [ProtoMember(1)] public ulong Uint64GroupCode { get; set; }
    
    [ProtoMember(2)] public uint Uint32AppId { get; set; }
    
    [ProtoMember(3)] public uint Uint32BusId { get; set; }
    
    [ProtoMember(4)] public string StrParentFolderId { get; set; }
    
    [ProtoMember(5)] public string StrFileId { get; set; }
    
    [ProtoMember(6)] public uint Uint32MsgdbSeq { get; set; }
    
    [ProtoMember(7)] public uint Uint32MsgRand { get; set; }
}

[ProtoPackable]
internal partial class DeleteFileRspBody
{
    [ProtoMember(1)] public long Int32RetCode { get; set; }
    
    [ProtoMember(2)] public string StrRetMsg { get; set; }
    
    [ProtoMember(3)] public string StrClientWording { get; set; }
}

[ProtoPackable]
internal partial class DownloadFileReqBody
{
    [ProtoMember(1)] public ulong Uint64GroupCode { get; set; }
    
    [ProtoMember(2)] public uint Uint32AppId { get; set; }
    
    [ProtoMember(3)] public uint Uint32BusId { get; set; }
    
    [ProtoMember(4)] public string StrFileId { get; set; }
    
    [ProtoMember(5)] public bool BoolThumbnailReq { get; set; }
    
    [ProtoMember(6)] public uint Uint32UrlType { get; set; }
    
    [ProtoMember(7)] public bool BoolPreviewReq { get; set; }
    
    [ProtoMember(8)] public uint Uint32Src { get; set; }
}

[ProtoPackable]
internal partial class DownloadFileRspBody
{
    [ProtoMember(1)] public long Int32RetCode { get; set; }
    
    [ProtoMember(2)] public string StrRetMsg { get; set; }
    
    [ProtoMember(3)] public string StrClientWording { get; set; }
    
    [ProtoMember(4)] public string StrDownloadIp { get; set; }
    
    [ProtoMember(5)] public string StrDownloadDns { get; set; }
    
    [ProtoMember(6)] public byte[] BytesDownloadUrl { get; set; }
    
    [ProtoMember(7)] public byte[] BytesSha { get; set; }
    
    [ProtoMember(8)] public byte[] BytesSha3 { get; set; }
    
    [ProtoMember(9)] public byte[] BytesMd5 { get; set; }
    
    [ProtoMember(10)] public byte[] BytesCookieVal { get; set; }
    
    [ProtoMember(11)] public string StrSaveFileName { get; set; }
    
    [ProtoMember(12)] public uint Uint32PreviewPort { get; set; }
    
    [ProtoMember(13)] public string StrDownloadDnsHttps { get; set; }
    
    [ProtoMember(14)] public uint Uint32PreviewPortHttps { get; set; }
}

[ProtoPackable]
internal partial class MoveFileReqBody
{
    [ProtoMember(1)] public ulong Uint64GroupCode { get; set; }
    
    [ProtoMember(2)] public uint Uint32AppId { get; set; }
    
    [ProtoMember(3)] public uint Uint32BusId { get; set; }
    
    [ProtoMember(4)] public string StrFileId { get; set; }
    
    [ProtoMember(5)] public string StrParentFolderId { get; set; }
    
    [ProtoMember(6)] public string StrDestFolderId { get; set; }
}

[ProtoPackable]
internal partial class MoveFileRspBody
{
    [ProtoMember(1)] public long Int32RetCode { get; set; }
    
    [ProtoMember(2)] public string StrRetMsg { get; set; }
    
    [ProtoMember(3)] public string StrClientWording { get; set; }
    
    [ProtoMember(4)] public string StrParentFolderId { get; set; }
}

[ProtoPackable]
internal partial class RenameFileReqBody
{
    [ProtoMember(1)] public ulong Uint64GroupCode { get; set; }
    
    [ProtoMember(2)] public uint Uint32AppId { get; set; }
    
    [ProtoMember(3)] public uint Uint32BusId { get; set; }
    
    [ProtoMember(4)] public string StrFileId { get; set; }
    
    [ProtoMember(5)] public string StrParentFolderId { get; set; }
    
    [ProtoMember(6)] public string StrNewFileName { get; set; }
}

[ProtoPackable]
internal partial class RenameFileRspBody
{
    [ProtoMember(1)] public long Int32RetCode { get; set; }
    
    [ProtoMember(2)] public string StrRetMsg { get; set; }
    
    [ProtoMember(3)] public string StrClientWording { get; set; }
}

[ProtoPackable]
internal partial class D6D6ReqBody
{
    [ProtoMember(1)] public UploadFileReqBody UploadFileReq { get; set; }
    
    [ProtoMember(2)] public ResendReqBody ResendFileReq { get; set; }
    
    [ProtoMember(3)] public DownloadFileReqBody DownloadFileReq { get; set; }
    
    [ProtoMember(4)] public DeleteFileReqBody DeleteFileReq { get; set; }
    
    [ProtoMember(5)] public RenameFileReqBody RenameFileReq { get; set; }
    
    [ProtoMember(6)] public MoveFileReqBody MoveFileReq { get; set; }
}

[ProtoPackable]
internal partial class ResendReqBody
{
    [ProtoMember(1)] public ulong Uint64GroupCode { get; set; }
    
    [ProtoMember(2)] public uint Uint32AppId { get; set; }
    
    [ProtoMember(3)] public uint Uint32BusId { get; set; }
    
    [ProtoMember(4)] public string StrFileId { get; set; }
    
    [ProtoMember(5)] public byte[] BytesSha { get; set; }
}

[ProtoPackable]
internal partial class ResendRspBody
{
    [ProtoMember(1)] public long Int32RetCode { get; set; }
    
    [ProtoMember(2)] public string StrRetMsg { get; set; }
    
    [ProtoMember(3)] public string StrClientWording { get; set; }
    
    [ProtoMember(4)] public string StrUploadIp { get; set; }
    
    [ProtoMember(5)] public byte[] BytesFileKey { get; set; }
    
    [ProtoMember(6)] public byte[] BytesCheckKey { get; set; }
}

[ProtoPackable]
internal partial class D6D6RspBody
{
    [ProtoMember(1)] public UploadFileRspBody UploadFileRsp { get; set; }
    
    [ProtoMember(2)] public ResendRspBody ResendFileRsp { get; set; }
    
    [ProtoMember(3)] public DownloadFileRspBody DownloadFileRsp { get; set; }
    
    [ProtoMember(4)] public DeleteFileRspBody DeleteFileRsp { get; set; }
    
    [ProtoMember(5)] public RenameFileRspBody RenameFileRsp { get; set; }
    
    [ProtoMember(6)] public MoveFileRspBody MoveFileRsp { get; set; }
}

[ProtoPackable]
internal partial class UploadFileReqBody
{
    [ProtoMember(1)] public ulong Uint64GroupCode { get; set; }
    
    [ProtoMember(2)] public uint Uint32AppId { get; set; }
    
    [ProtoMember(3)] public uint Uint32BusId { get; set; }
    
    [ProtoMember(4)] public uint Uint32Entrance { get; set; }
    
    [ProtoMember(5)] public string StrParentFolderId { get; set; }
    
    [ProtoMember(6)] public string StrFileName { get; set; }
    
    [ProtoMember(7)] public string StrLocalPath { get; set; }
    
    [ProtoMember(8)] public ulong Uint64FileSize { get; set; }
    
    [ProtoMember(9)] public byte[] BytesSha { get; set; }
    
    [ProtoMember(10)] public byte[] BytesSha3 { get; set; }
    
    [ProtoMember(11)] public byte[] BytesMd5 { get; set; }
    
    [ProtoMember(12)] public bool BoolSupportMultiUpload { get; set; }
}

[ProtoPackable]
internal partial class UploadFileRspBody
{
    [ProtoMember(1)] public long Int32RetCode { get; set; }
    
    [ProtoMember(2)] public string StrRetMsg { get; set; }
    
    [ProtoMember(3)] public string StrClientWording { get; set; }
    
    [ProtoMember(4)] public string StrUploadIp { get; set; }
    
    [ProtoMember(5)] public string StrServerDns { get; set; }
    
    [ProtoMember(6)] public uint Uint32BusId { get; set; }
    
    [ProtoMember(7)] public string StrFileId { get; set; }
    
    [ProtoMember(8)] public byte[] BytesFileKey { get; set; }
    
    [ProtoMember(9)] public byte[] BytesCheckKey { get; set; }
    
    [ProtoMember(10)] public bool BoolFileExist { get; set; }
    
    [ProtoMember(12)] public List<string> StrUploadIpLanV4 { get; set; }
    
    [ProtoMember(13)] public List<string> StrUploadIpLanV6 { get; set; }
    
    [ProtoMember(14)] public uint Uint32UploadPort { get; set; }
}