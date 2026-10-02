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
    
    private string? _fallbackUrl;
    
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

        try
        {
            NTV2RichMediaDownloadEventResp result = message.IsGroup()
                ? await context.EventContext.SendEvent<ImageGroupDownloadEventResp>(new ImageGroupDownloadEventReq(message, this))
                : await context.EventContext.SendEvent<ImageDownloadEventResp>(new ImageDownloadEventReq(message, this));
            
            FileUrl = result.Url;
        }
        catch
        {
            if (!string.IsNullOrEmpty(_fallbackUrl))
            {
                FileUrl = BuildUrl(_fallbackUrl);
            }
        }
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
            uint compatSize = 0;
            foreach (var elem in elements)
            {
                if (elem.CustomFace is { Md5: { Length: > 0 } faceMd5 } compatFace &&
                    Convert.ToHexString(faceMd5).Equals(info.FileHash, StringComparison.OrdinalIgnoreCase))
                {
                    compatUrl = compatFace.OrigUrl;
                    compatSize = compatFace.Size;
                    break;
                }
                if (elem.NotOnlineImage is { PicMd5: { Length: > 0 } picMd5 } compatImage &&
                    Convert.ToHexString(picMd5).Equals(info.FileHash, StringComparison.OrdinalIgnoreCase))
                {
                    compatUrl = compatImage.OrigUrl;
                    compatSize = compatImage.FileLen;
                    break;
                }
            }

            var picExt = msgInfo.ExtBizInfo.Pic;
            var entity = new ImageEntity
            {
                MsgInfo = msgInfo,
                _fallbackUrl = compatUrl,
                FileUrl = string.Empty,
                ImageSize = new Vector2(info.Width, info.Height),
                SubType = ResolveSubType(picExt),
                Summary = string.IsNullOrEmpty(picExt.TextSummary)
                    ? (ResolveSubType(picExt) == 1 ? "[动画表情]" : "[图片]")
                    : picExt.TextSummary,
            };

            if (entity.FileSize == 0 && compatSize != 0)
            {
                entity.FileSize = compatSize;
            }

            return entity;
        }

        if (target.NotOnlineImage is { } image)
        {
            string md5 = image.PicMd5 is { Length: > 0 } ? Convert.ToHexString(image.PicMd5) : string.Empty;
            if (!string.IsNullOrEmpty(md5) && HasMatchingCommonElem(elements, md5))
            {
                return null;
            }

            return new ImageEntity
            {
                ImageSize = new Vector2(image.PicWidth, image.PicHeight),
                FileSize = image.FileLen,
                FileUrl = string.IsNullOrEmpty(image.OrigUrl) ? string.Empty : BuildUrl(image.OrigUrl),
                SubType = (int)image.BizType,
            };
        }

        if (target.CustomFace is { } face)
        {
            string md5 = face.Md5 is { Length: > 0 } ? Convert.ToHexString(face.Md5) : string.Empty;
            if (!string.IsNullOrEmpty(md5) && HasMatchingCommonElem(elements, md5))
            {
                return null;
            }

            return new ImageEntity
            {
                ImageSize = new Vector2(face.Width, face.Height),
                FileSize = face.Size,
                FileUrl = string.IsNullOrEmpty(face.OrigUrl) ? string.Empty : BuildUrl(face.OrigUrl),
                SubType = face.BizType,
            };
        }

        return null;
    }

    private static int ResolveSubType(PicExtBizInfo picExt)
    {
        if (picExt.BizType != 0) return (int)picExt.BizType;

        foreach (var reserve in new[] { picExt.BytesPbReserveTroop, picExt.BytesPbReserveC2c })
        {
            if (reserve is not { Length: > 0 }) continue;

            try
            {
                var parsed = ProtoHelper.Deserialize<PicExtBizInfoReserve>(reserve);
                if (parsed.SubType != 0) return (int)parsed.SubType;
            }
            catch
            {
                // ignored
            }
        }

        return 0;
    }

    private static bool HasMatchingCommonElem(List<Elem> elements, string fileHash)
    {
        foreach (var elem in elements)
        {
            if (elem.CommonElem is { BusinessType: 10 or 20 } ce)
            {
                try
                {
                    var msgInfo = ProtoHelper.Deserialize<MsgInfo>(ce.PbElem.Span);
                    if (msgInfo.MsgInfoBody.Count > 0 &&
                        msgInfo.MsgInfoBody[0].Index?.Info?.FileHash is { } hash &&
                        hash.Equals(fileHash, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        return false;
    }

    private static string BuildUrl(string origUrl) =>
        origUrl.Contains("&fileid=") ? $"{BaseUrl}{origUrl}" : $"{LegacyBaseUrl}{origUrl}";
}