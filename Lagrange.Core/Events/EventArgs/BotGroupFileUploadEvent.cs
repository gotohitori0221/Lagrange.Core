namespace Lagrange.Core.Events.EventArgs;

public class BotGroupFileUploadEvent(long groupUin, long senderUin, string fileName, ulong fileSize, string fileId) : EventBase
{
    public long GroupUin { get; } = groupUin;

    public long SenderUin { get; } = senderUin;

    public string FileName { get; } = fileName;

    public ulong FileSize { get; } = fileSize;

    public string FileId { get; } = fileId;

    public override string ToEventMessage()
    {
        return $"{nameof(BotGroupFileUploadEvent)}: Group={GroupUin}, from={SenderUin}, file={FileName}, size={FileSize}";
    }
}
