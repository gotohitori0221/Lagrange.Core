using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Message;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Extension;

namespace Lagrange.Core.Message.Entities;

public class ImageEntity : RichMediaEntityBase
{
    private const string BaseUrl = "https://multimedia.nt.qq.com.cn";

    private const string LegacyBaseUrl = "http://gchat.qpic.cn";

    internal override Lazy<Stream>? Stream { get; }
    
    public Vector2 ImageSize { get; set; }
    
    public int SubType { get; init; }
    
    public string Summary { get; init; } = "[图片]";

    public ImageEntity() { }
    
    public ImageEntity(Stream stream, string? summary = "[图片]", int subType = 0, bool disposeOnCompletion = false)
    {
        Stream = new Lazy<Stream>(() => stream);
        Summary = summary ?? "[图片]";
        SubType = subType;
        DisposeOnCompletion = disposeOnCompletion;
    }
    
    public override async Task Preprocess(BotContext context, BotMessage message)
    { 
        ArgumentNullException.ThrowIfNull(Stream);

        try
        {
            IsGroup = message.IsGroup();
            NTV2RichMediaUploadEventResp result = IsGroup
                ? await context.EventContext.SendEvent<ImageGroupUploadEventResp>(new ImageGroupUploadEventReq(message, this))
                : await context.EventContext.SendEvent<ImageUploadEventResp>(new ImageUploadEventReq(message, this));

            _compat = result.Compat;
            MsgInfo = result.Info;

            if (result.Ext != null)
            {
                
                if (RuntimeFeature.IsDynamicCodeCompiled && !RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    await context.HighwayContext.UploadFile(Stream.Value, IsGroup ? 1004 : 1003, ProtoHelper.Serialize(result.Ext));
                }
                else
                {
                    await context.FlashTransferContext.UploadFile(result.Ext.UKey, (uint)(IsGroup ? 1407 : 1406), Stream.Value);
                }
            }
        }
        finally
        {
            if (DisposeOnCompletion) await Stream.Value.DisposeAsync();
        }
    }

    public override async Task Postprocess(BotContext context, BotMessage message)
    {
        if (!string.IsNullOrEmpty(FileUrl)) return;

        NTV2RichMediaDownloadEventResp result = message.IsGroup()
            ? await context.EventContext.SendEvent<ImageGroupDownloadEventResp>(new ImageGroupDownloadEventReq(message, this))
            : await context.EventContext.SendEvent<ImageDownloadEventResp>(new ImageDownloadEventReq(message, this));
        
        FileUrl = result.Url;
    }

    public override string ToPreviewString() =>
        string.IsNullOrEmpty(FileUrl)
            ? $"[Image: {ImageSize.X}x{ImageSize.Y}] {Summary} {FileSize} bytes"
            : $"[Image: {ImageSize.X}x{ImageSize.Y}] {Summary} {FileSize} bytes | URL: {FileUrl}";
    internal override Elem[] Build()
    {
        if (_compat != null)
        {
            var compatElem = IsGroup
                ? new Elem { CustomFace = ProtoHelper.Deserialize<CustomFace>(_compat) }
                : new Elem { NotOnlineImage = ProtoHelper.Deserialize<NotOnlineImage>(_compat) };
            
            return
            [
                compatElem,
                new Elem()
                {
                    CommonElem = new CommonElem
                    {
                        ServiceType = 48,
                        PbElem = ProtoHelper.Serialize(MsgInfo ?? throw new ArgumentNullException(nameof(MsgInfo))),
                        BusinessType = IsGroup ? 20u : 10u,
                    }
                }
            ];
        }
        else
        {
            return
            [
                new Elem()
                {
                    CommonElem = new CommonElem
                    {
                        ServiceType = 48,
                        PbElem = ProtoHelper.Serialize(MsgInfo ?? throw new ArgumentNullException(nameof(MsgInfo))),
                        BusinessType = IsGroup ? 20u : 10u,
                    }
                }
            ];
        }
    }

    internal override IMessageEntity? Parse(List<Elem> elements, Elem target)
    {
        if (target.CommonElem is { BusinessType: 10 or 20 } commonElem)
        {
            var msgInfo = ProtoHelper.Deserialize<MsgInfo>(commonElem.PbElem.Span);
            var info = msgInfo.MsgInfoBody[0].Index.Info;

            string? compatUrl = null;
            foreach (var elem in elements)
            {
                if (elem.CustomFace is { Md5: { Length: > 0 } faceMd5 } compatFace &&
                    Convert.ToHexString(faceMd5).Equals(info.FileHash, StringComparison.OrdinalIgnoreCase))
                {
                    compatUrl = compatFace.OrigUrl;
                    break;
                }
                if (elem.NotOnlineImage is { PicMd5: { Length: > 0 } picMd5 } compatImage &&
                    Convert.ToHexString(picMd5).Equals(info.FileHash, StringComparison.OrdinalIgnoreCase))
                {
                    compatUrl = compatImage.OrigUrl;
                    break;
                }
            }

            return new ImageEntity
            {
                MsgInfo = msgInfo,
                FileUrl = string.IsNullOrEmpty(compatUrl) ? string.Empty : BuildUrl(compatUrl),
                ImageSize = new Vector2(info.Width, info.Height),
                SubType = (int)msgInfo.ExtBizInfo.Pic.BizType,
                Summary = string.IsNullOrEmpty(msgInfo.ExtBizInfo.Pic.TextSummary) ? "[图片]" : msgInfo.ExtBizInfo.Pic.TextSummary,
            };
        }

        if (target.NotOnlineImage is { } image)
        {
            return new ImageEntity
            {
                ImageSize = new Vector2(image.PicWidth, image.PicHeight),
                FileUrl = string.IsNullOrEmpty(image.OrigUrl) ? string.Empty : BuildUrl(image.OrigUrl),
                SubType = (int)image.BizType,
            };
        }

        if (target.CustomFace is { } face)
        {
            return new ImageEntity
            {
                ImageSize = new Vector2(face.Width, face.Height),
                FileUrl = string.IsNullOrEmpty(face.OrigUrl) ? string.Empty : BuildUrl(face.OrigUrl),
                SubType = face.BizType,
            };
        }

        return null;
    }

    private static string BuildUrl(string origUrl) =>
        origUrl.Contains("&fileid=") ? $"{BaseUrl}{origUrl}" : $"{LegacyBaseUrl}{origUrl}";
}