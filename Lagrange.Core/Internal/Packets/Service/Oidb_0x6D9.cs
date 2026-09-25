using Lagrange.Core.Internal.Packets.Message;
using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class CopyFromReqBody
{
    [ProtoMember(1)] public ulong GroupCode { get; set; }
    
    [ProtoMember(2)] public uint AppId { get; set; }
    
    [ProtoMember(3)] public uint SrcBusId { get; set; }
    
    [ProtoMember(4)] public byte[] SrcParentFolder { get; set; }
    
    [ProtoMember(5)] public byte[] SrcFilePath { get; set; }
    
    [ProtoMember(6)] public uint DstBusId { get; set; }
    
    [ProtoMember(7)] public byte[] DstFolderId { get; set; }
    
    [ProtoMember(8)] public ulong FileSize { get; set; }
    
    [ProtoMember(9)] public string LocalPath { get; set; }
    
    [ProtoMember(10)] public string FileName { get; set; }
    
    [ProtoMember(11)] public ulong SrcUin { get; set; }
    
    [ProtoMember(12)] public byte[] Md5 { get; set; }
}

[ProtoPackable]
internal partial class CopyFromRspBody
{
    [ProtoMember(1)] public long RetCode { get; set; }
    
    [ProtoMember(2)] public string RetMsg { get; set; }
    
    [ProtoMember(3)] public string ClientWording { get; set; }
    
    [ProtoMember(4)] public byte[] SaveFilePath { get; set; }
    
    [ProtoMember(5)] public uint BusId { get; set; }
}

[ProtoPackable]
internal partial class CopyToReqBody
{
    [ProtoMember(1)] public ulong GroupCode { get; set; }
    
    [ProtoMember(2)] public uint AppId { get; set; }
    
    [ProtoMember(3)] public uint SrcBusId { get; set; }
    
    [ProtoMember(4)] public string SrcFileId { get; set; }
    
    [ProtoMember(5)] public uint DstBusId { get; set; }
    
    [ProtoMember(6)] public ulong DstUin { get; set; }
    
    [ProtoMember(40)] public string NewFileName { get; set; }
    
    [ProtoMember(100)] public byte[] TimCloudPdirKey { get; set; }
    
    [ProtoMember(101)] public byte[] TimCloudPpdirKey { get; set; }
    
    [ProtoMember(102)] public byte[] TimCloudExtensionInfo { get; set; }
    
    [ProtoMember(103)] public uint TimFileExistOption { get; set; }
}

[ProtoPackable]
internal partial class CopyToRspBody
{
    [ProtoMember(1)] public long RetCode { get; set; }
    
    [ProtoMember(2)] public string RetMsg { get; set; }
    
    [ProtoMember(3)] public string ClientWording { get; set; }
    
    [ProtoMember(4)] public string SaveFilePath { get; set; }
    
    [ProtoMember(5)] public uint BusId { get; set; }
    
    [ProtoMember(40)] public string FileName { get; set; }
}

[ProtoPackable]
internal partial class FeedsReqBody
{
    [ProtoMember(1)] public ulong GroupCode { get; set; }
    
    [ProtoMember(2)] public uint AppId { get; set; }
    
    [ProtoMember(3)] public List<FeedsInfo> FeedsInfoList { get; set; }
    
    [ProtoMember(4)] public uint MultiSendSeq { get; set; }
}

[ProtoPackable]
internal partial class FeedsRspBody
{
    [ProtoMember(1)] public long RetCode { get; set; }
    
    [ProtoMember(2)] public string RetMsg { get; set; }
    
    [ProtoMember(3)] public string ClientWording { get; set; }
    
    [ProtoMember(4)] public List<FeedsResult> FeedsResultList { get; set; }
    
    [ProtoMember(5)] public uint SvrBusyWaitTime { get; set; }
}

[ProtoPackable]
internal partial class D6D9ReqBody
{
    [ProtoMember(1)] public TransFileReqBody TransFileReq { get; set; }
    
    [ProtoMember(2)] public CopyFromReqBody CopyFromReq { get; set; }
    
    [ProtoMember(3)] public CopyToReqBody CopyToReq { get; set; }
    
    [ProtoMember(5)] public FeedsReqBody FeedsInfoReq { get; set; }
}

[ProtoPackable]
internal partial class D6D9RspBody
{
    [ProtoMember(1)] public TransFileRspBody TransFileRsp { get; set; }
    
    [ProtoMember(2)] public CopyFromRspBody CopyFromRsp { get; set; }
    
    [ProtoMember(3)] public CopyToRspBody CopyToRsp { get; set; }
    
    [ProtoMember(5)] public FeedsRspBody FeedsInfoRsp { get; set; }
}

[ProtoPackable]
internal partial class TransFileReqBody
{
    [ProtoMember(1)] public ulong GroupCode { get; set; }
    
    [ProtoMember(2)] public uint AppId { get; set; }
    
    [ProtoMember(3)] public uint BusId { get; set; }
    
    [ProtoMember(4)] public string FileId { get; set; }
}

[ProtoPackable]
internal partial class TransFileRspBody
{
    [ProtoMember(1)] public long RetCode { get; set; }
    
    [ProtoMember(2)] public string RetMsg { get; set; }
    
    [ProtoMember(3)] public string ClientWording { get; set; }
    
    [ProtoMember(4)] public uint SaveBusId { get; set; }
    
    [ProtoMember(5)] public string SaveFilePath { get; set; }
}