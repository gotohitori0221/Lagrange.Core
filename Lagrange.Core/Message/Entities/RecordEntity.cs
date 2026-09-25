using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Message;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Extension;

namespace Lagrange.Core.Message.Entities;

public class RecordEntity : RichMediaEntityBase
{
    internal override Lazy<Stream>? Stream { get; }
    
    public uint RecordLength { get; set; }
    
    public RecordEntity() { }
    
    public RecordEntity(Stream stream, bool disposeOnCompletion = false)
    {
        Stream = new Lazy<Stream>(() => stream, disposeOnCompletion);
        DisposeOnCompletion = disposeOnCompletion;
    }
    
    public override async Task Preprocess(BotContext context, BotMessage message)
    {
        ArgumentNullException.ThrowIfNull(Stream);

        try
        {
            IsGroup = message.IsGroup();
            NTV2RichMediaUploadEventResp result = IsGroup
                ? await context.EventContext.SendEvent<RecordGroupUploadEventResp>(new RecordGroupUploadEventReq(message, this))
                : await context.EventContext.SendEvent<RecordUploadEventResp>(new RecordUploadEventReq(message, this));

            _compat = result.Compat;
            MsgInfo = result.Info;

            if (result.Ext != null)
            {
                await context.HighwayContext.UploadFile(Stream.Value, message.IsGroup() ? 1008 : 1007, ProtoHelper.Serialize(result.Ext));
            }
        }  
        finally
        {
            if (DisposeOnCompletion) await Stream.Value.DisposeAsync();
        }
    }

    public override async Task Postprocess(BotContext context, BotMessage message)
    {
        if (string.IsNullOrEmpty(FileUrl))
        {
            NTV2RichMediaDownloadEventResp result = message.IsGroup()
                ? await context.EventContext.SendEvent<RecordGroupDownloadEventResp>(new RecordGroupDownloadEventReq(message, this))
                : await context.EventContext.SendEvent<RecordDownloadEventResp>(new RecordDownloadEventReq(message, this));

            FileUrl = result.Url;
        }
    }

    public override string ToPreviewString() => $"[语音] {RecordLength}\" {FileUrl}";

    internal override Elem[] Build() =>
    [
        new()
        {
            CommonElem = new CommonElem
            {
                ServiceType = 48,
                PbElem = ProtoHelper.Serialize(MsgInfo ?? throw new ArgumentNullException(nameof(MsgInfo))),
                BusinessType = IsGroup ? 22u : 12u,
            }
        }
    ];

    internal override IMessageEntity? Parse(List<Elem> elements, Elem target)
    {
        if (target.CommonElem is { BusinessType: 12 or 22 } commonElem)
        {
            var msgInfo = ProtoHelper.Deserialize<MsgInfo>(commonElem.PbElem.Span);
            var info = msgInfo.MsgInfoBody[0].Index.Info;
            
            return new RecordEntity
            {
                MsgInfo = msgInfo,
                RecordLength = info.Time
            };
        }
        
        return null;
    }
}