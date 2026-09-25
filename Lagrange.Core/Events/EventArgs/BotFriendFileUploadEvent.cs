namespace Lagrange.Core.Events.EventArgs;

public class BotFriendFileUploadEvent(long senderUin, string fileName, ulong fileSize, string fileId, bool isSelf, string fileHash) : EventBase
{
    public long SenderUin { get; } = senderUin;

    public string FileName { get; } = fileName;

    public ulong FileSize { get; } = fileSize;

    public string FileId { get; } = fileId;

    public bool IsSelf { get; } = isSelf;

    public string FileHash { get; } = fileHash;

    public override string ToEventMessage() =>
        $"{nameof(BotFriendFileUploadEvent)}: from={SenderUin}, file={FileName}, size={FileSize}";
}
