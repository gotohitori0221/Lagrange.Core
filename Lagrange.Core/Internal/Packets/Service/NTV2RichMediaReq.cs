using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;


#pragma warning disable CS8618

[ProtoPackable]
internal partial class NTV2RichMediaReq
{
    [ProtoMember(1)] public MultiMediaReqHead ReqHead { get; set; }

    [ProtoMember(2)] public UploadReq Upload { get; set; }

    [ProtoMember(3)] public DownloadReq Download { get; set; }

    [ProtoMember(4)] public DownloadRKeyReq DownloadRKey { get; set; }

    [ProtoMember(5)] public DeleteReq Delete { get; set; }

    [ProtoMember(6)] public UploadCompletedReq UploadCompleted { get; set; }

    [ProtoMember(7)] public MsgInfoAuthReq MsgInfoAuth { get; set; }

    [ProtoMember(8)] public UploadKeyRenewalReq UploadKeyRenewal { get; set; }

    [ProtoMember(9)] public DownloadSafeReq DownloadSafe { get; set; }

    [ProtoMember(99)] public byte[]? Extension { get; set; }
}

[ProtoPackable]
internal partial class MultiMediaReqHead
{
    [ProtoMember(1)] public CommonHead Common { get; set; }

    [ProtoMember(2)] public SceneInfo Scene { get; set; }

    [ProtoMember(3)] public ClientMeta Client { get; set; }
}

[ProtoPackable]
internal partial class CommonHead
{
    [ProtoMember(1)] public uint RequestId { get; set; } 

    [ProtoMember(2)] public uint Command { get; set; } 
}

[ProtoPackable]
internal partial class SceneInfo
{
    [ProtoMember(101)] public uint RequestType { get; set; } 

    [ProtoMember(102)] public uint BusinessType { get; set; } 

    [ProtoMember(200)] public uint SceneType { get; set; } 

    [ProtoMember(201)] public C2CUserInfo? C2C { get; set; }


    [ProtoMember(202)] public GroupInfo? Group { get; set; }
}

[ProtoPackable]
internal partial class C2CUserInfo
{
    [ProtoMember(1)] public uint AccountType { get; set; } 

    [ProtoMember(2)] public string TargetUid { get; set; }
}


[ProtoPackable]
internal partial class GroupInfo
{
    [ProtoMember(1)] public long GroupUin { get; set; }
}

[ProtoPackable]
internal partial class ClientMeta
{
    [ProtoMember(1)] public uint AgentType { get; set; } 
}

[ProtoPackable]
internal partial class DownloadReq
{
    [ProtoMember(1)] public IndexNode Node { get; set; }

    [ProtoMember(2)] public DownloadExt Download { get; set; }
}

[ProtoPackable]
internal partial class IndexNode
{
    [ProtoMember(1)] public FileInfo Info { get; set; }

    [ProtoMember(2)] public string FileUuid { get; set; }

    [ProtoMember(3)] public uint StoreId { get; set; } 

    [ProtoMember(4)] public uint UploadTime { get; set; } 

    [ProtoMember(5)] public uint Ttl { get; set; } 

    [ProtoMember(6)] public uint SubType { get; set; } 
}

[ProtoPackable]
internal partial class FileInfo
{
    [ProtoMember(1)] public uint FileSize { get; set; } 

    [ProtoMember(2)] public string FileHash { get; set; }

    [ProtoMember(3)] public string FileSha1 { get; set; } 

    [ProtoMember(4)] public string FileName { get; set; }

    [ProtoMember(5)] public FileType Type { get; set; }

    [ProtoMember(6)] public uint Width { get; set; } 

    [ProtoMember(7)] public uint Height { get; set; } 

    [ProtoMember(8)] public uint Time { get; set; } 

    [ProtoMember(9)] public uint Original { get; set; } 
}

[ProtoPackable]
internal partial class FileType
{
    [ProtoMember(1)] public uint Type { get; set; } 

    [ProtoMember(2)] public uint PicFormat { get; set; } 

    [ProtoMember(3)] public uint VideoFormat { get; set; } 

    [ProtoMember(4)] public uint VoiceFormat { get; set; } 
}

[ProtoPackable]
internal partial class DownloadExt
{
    [ProtoMember(1)] public PicDownloadExt Pic { get; set; }

    [ProtoMember(2)] public VideoDownloadExt Video { get; set; }

    [ProtoMember(3)] public PttDownloadExt Ptt { get; set; }
}

[ProtoPackable]
internal partial class VideoDownloadExt
{
    [ProtoMember(1)] public uint BusiType { get; set; } 

    [ProtoMember(2)] public uint SceneType { get; set; } 

    [ProtoMember(3)] public uint SubBusiType { get; set; } 
}

[ProtoPackable]
internal partial class PicDownloadExt { }

[ProtoPackable]
internal partial class PttDownloadExt { }

[ProtoPackable]
internal partial class PicUrlExtInfo
{
    [ProtoMember(1)] public string OriginalParameter { get; set; }

    [ProtoMember(2)] public string BigParameter { get; set; }

    [ProtoMember(3)] public string ThumbParameter { get; set; }
}

[ProtoPackable]
internal partial class VideoExtInfo
{
    [ProtoMember(1)] public uint VideoCodecFormat { get; set; }
}

[ProtoPackable]
internal partial class MsgInfo
{
    [ProtoMember(1)] public List<MsgInfoBody> MsgInfoBody { get; set; }

    [ProtoMember(2)] public ExtBizInfo ExtBizInfo { get; set; }
}

[ProtoPackable]
internal partial class MsgInfoBody
{
    [ProtoMember(1)] public IndexNode Index { get; set; }

    [ProtoMember(2)] public PictureInfo Picture { get; set; }

    [ProtoMember(3)] public VideoInfo Video { get; set; }

    [ProtoMember(4)] public AudioInfo Audio { get; set; }

    [ProtoMember(5)] public bool FileExist { get; set; }

    [ProtoMember(6)] public HashSum HashSum { get; set; }
}

[ProtoPackable]
internal partial class HashSum
{
    [ProtoMember(201)] public C2cSource BytesPbReserveC2c;

    [ProtoMember(202)] public TroopSource? TroopSource;
}

[ProtoPackable]
internal partial class C2cSource
{
    [ProtoMember(2)] public string FriendUid;
}

[ProtoPackable]
internal partial class TroopSource
{
    [ProtoMember(1)] public uint GroupUin;
}

[ProtoPackable]
internal partial class VideoInfo { }

[ProtoPackable]
internal partial class AudioInfo { }

[ProtoPackable]
internal partial class PictureInfo
{
    [ProtoMember(1)] public string UrlPath { get; set; }

    [ProtoMember(2)] public PicUrlExtInfo Ext { get; set; }

    [ProtoMember(3)] public string Domain { get; set; }
}


[ProtoPackable]
internal partial class ExtBizInfo
{
    [ProtoMember(1)] public PicExtBizInfo Pic { get; set; }

    [ProtoMember(2)] public VideoExtBizInfo Video { get; set; }

    [ProtoMember(3)] public PttExtBizInfo Ptt { get; set; }

    [ProtoMember(10)] public uint BusiType { get; set; }
}

[ProtoPackable]
internal partial class PttExtBizInfo
{
    [ProtoMember(1)] public ulong SrcUin { get; set; }

    [ProtoMember(2)] public uint PttScene { get; set; }

    [ProtoMember(3)] public uint PttType { get; set; }

    [ProtoMember(4)] public uint ChangeVoice { get; set; }

    [ProtoMember(5)] public byte[] Waveform { get; set; }

    [ProtoMember(6)] public uint AutoConvertText { get; set; }

    [ProtoMember(11)] public byte[] BytesReserve { get; set; }

    [ProtoMember(12)] public byte[] BytesPbReserve { get; set; }

    [ProtoMember(13)] public byte[] BytesGeneralFlags { get; set; }
}

[ProtoPackable]
internal partial class VideoExtBizInfo
{
    [ProtoMember(1)] public uint FromScene { get; set; }

    [ProtoMember(2)] public uint ToScene { get; set; }

    [ProtoMember(3)] public byte[] BytesPbReserve { get; set; }
}

[ProtoPackable]
internal partial class PicExtBizInfo
{
    [ProtoMember(1)] public uint BizType { get; set; }

    [ProtoMember(2)] public string TextSummary { get; set; }

    [ProtoMember(11)] public byte[]? BytesPbReserveC2c { get; set; }

    [ProtoMember(12)] public byte[]? BytesPbReserveTroop { get; set; }

    [ProtoMember(1001)] public uint FromScene { get; set; }

    [ProtoMember(1002)] public uint ToScene { get; set; }

    [ProtoMember(1003)] public uint OldFileId { get; set; }
}

[ProtoPackable]
internal partial class DownloadSafeReq
{
    [ProtoMember(1)] public IndexNode Index { get; set; }
}

[ProtoPackable]
internal partial class UploadKeyRenewalReq
{
    [ProtoMember(1)] public string OldUKey { get; set; }

    [ProtoMember(2)] public uint SubType { get; set; }
}

[ProtoPackable]
internal partial class MsgInfoAuthReq
{
    [ProtoMember(1)] public byte[] Msg { get; set; }

    [ProtoMember(2)] public ulong AuthTime { get; set; }
}

[ProtoPackable]
internal partial class UploadCompletedReq
{
    [ProtoMember(1)] public bool SrvSendMsg { get; set; }

    [ProtoMember(2)] public ulong ClientRandomId { get; set; }

    [ProtoMember(3)] public MsgInfo MsgInfo { get; set; }

    [ProtoMember(4)] public uint ClientSeq { get; set; }
}

[ProtoPackable]
internal partial class DeleteReq
{
    [ProtoMember(1)] public List<IndexNode> Index { get; set; }

    [ProtoMember(2)] public bool NeedRecallMsg { get; set; }

    [ProtoMember(3)] public ulong MsgSeq { get; set; }

    [ProtoMember(4)] public ulong MsgRandom { get; set; }

    [ProtoMember(5)] public ulong MsgTime { get; set; }
}

[ProtoPackable]
internal partial class DownloadRKeyReq
{
    [ProtoMember(1)] public List<int> Types { get; set; }
}

[ProtoPackable]
internal partial class UploadInfo
{
    [ProtoMember(1)] public FileInfo FileInfo { get; set; }

    [ProtoMember(2)] public uint SubFileType { get; set; }
}

[ProtoPackable]
internal partial class UploadReq
{
    [ProtoMember(1)] public List<UploadInfo> UploadInfo { get; set; }

    [ProtoMember(2)] public bool TryFastUploadCompleted { get; set; }

    [ProtoMember(3)] public bool SrvSendMsg { get; set; }

    [ProtoMember(4)] public ulong ClientRandomId { get; set; }

    [ProtoMember(5)] public uint CompatQMsgSceneType { get; set; }

    [ProtoMember(6)] public ExtBizInfo ExtBizInfo { get; set; }

    [ProtoMember(7)] public uint ClientSeq { get; set; }

    [ProtoMember(8)] public bool NoNeedCompatMsg { get; set; }
}