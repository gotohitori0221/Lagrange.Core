using System.Diagnostics;
using Lagrange.Core.Internal.Packets.Message;
using Lagrange.Core.Internal.Packets.Service;

namespace Lagrange.Core.Message.Entities;

public abstract class RichMediaEntityBase : IMessageEntity
{
    private MsgInfo? _msgInfo;

    protected byte[]? _compat;

    private protected bool IsGroup;

    internal MsgInfo? MsgInfo
    {
        get => _msgInfo;
        set
        {
            Debug.Assert(value != null);

            var fileInfo = value.MsgInfoBody[0].Index.Info;
            FileUuid = value.MsgInfoBody[0].Index.FileUuid;
            FileMd5 = fileInfo.FileHash;
            FileSha1 = fileInfo.FileSha1;
            FileSize = fileInfo.FileSize;
            FileName = fileInfo.FileName;
            _msgInfo = value;
        }
    }

    private protected bool DisposeOnCompletion;

    internal abstract Lazy<Stream>? Stream { get; }

    public string FileUuid { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string FileSha1 { get; set; } = string.Empty;

    public uint FileSize { get; set; }

    public string FileMd5 { get; set; } = string.Empty;

    public abstract Task Preprocess(BotContext context, BotMessage message);

    public abstract Task Postprocess(BotContext context, BotMessage message);

    internal abstract Elem[] Build();

    internal abstract IMessageEntity? Parse(List<Elem> elements, Elem target);

    public abstract string ToPreviewString();

    Elem[] IMessageEntity.Build() => Build();

    IMessageEntity? IMessageEntity.Parse(List<Elem> elements, Elem target) => Parse(elements, target);

    string IMessageEntity.ToPreviewString() => ToPreviewString();
}