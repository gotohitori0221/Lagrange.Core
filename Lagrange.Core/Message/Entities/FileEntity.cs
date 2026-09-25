using Lagrange.Core.Internal.Packets.Message;

namespace Lagrange.Core.Message.Entities;

public class FileEntity : IMessageEntity
{
    public string FileId { get; internal init; } = string.Empty;

    public string FileName { get; internal init; } = string.Empty;

    public long FileSize { get; internal init; }

    public string FileMd5 { get; internal init; } = string.Empty;

    public string? FileHash { get; internal init; }

    public Task Postprocess(BotContext context, BotMessage message) => Task.CompletedTask;

    Elem[] IMessageEntity.Build() => throw new NotSupportedException();

    IMessageEntity? IMessageEntity.Parse(List<Elem> elements, Elem target) => null;

    public string ToPreviewString() => $"[文件 {FileName}]";
}