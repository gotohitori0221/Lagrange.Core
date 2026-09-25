using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Notify;

#pragma warning disable CS8618


[ProtoPackable]
internal partial class GroupFileUploadNotify
{
    [ProtoMember(3)] public ulong GroupUin { get; set; }

    [ProtoMember(4)] public ulong SenderUin { get; set; }

    [ProtoMember(5)] public GroupFileUploadFileInfo FileInfo { get; set; }
}

[ProtoPackable]
internal partial class GroupFileUploadFileInfo
{
    [ProtoMember(1)] public string FileId { get; set; }

    [ProtoMember(2)] public string FileName { get; set; }

    [ProtoMember(3)] public ulong FileSize { get; set; }
}
