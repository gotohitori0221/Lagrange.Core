using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entities;
using Lagrange.Milky.Extensions;
using Lagrange.Milky.Models.Messages;
using Lagrange.Milky.Models.Segments;

namespace Lagrange.Milky.Converters;

public partial class MilkyConverter
{
    public async Task<IReadOnlyList<IncomingSegmentBase>> ToIncomingSegmentsAsync(MessageChain chain, MessageType type, long ownerPeerUin, CancellationToken ct = default)
    {
        List<IncomingSegmentBase> result = new(chain.Count);
        long? replySourceUin = null; 
        foreach (var entity in chain)
        {
            
            
            if (replySourceUin.HasValue && entity is MentionEntity mention && mention.Uin == replySourceUin.Value)
            {
                replySourceUin = null;
                continue;
            }
            replySourceUin = null;

            if (entity is ReplyEntity reply && reply.Source is BotGroupMember replyMember)
                replySourceUin = replyMember.Uin;

            var segment = await ToIncomingSegmentAsync(entity, type, ownerPeerUin, ct);
            if (segment != null) result.Add(segment);
        }
        return result;
    }

    private async Task<IncomingSegmentBase?> ToIncomingSegmentAsync(IMessageEntity entity, MessageType type, long ownerPeerUin, CancellationToken ct = default) => entity switch
    {
        TextEntity text => new TextIncomingSegment { Data = new TextIncomingSegmentData { Text = text.Text } },
        MentionEntity mention => mention.Uin == 0
            ? new MentionAllIncomingSegment { Data = new object(), }
            : new MentionIncomingSegment
            {
                Data = new MentionIncomingSegmentData
                {
                    UserId = mention.Uin,
                    Name = mention.Display ?? string.Empty,
                }
            },
        FaceEntity face => new FaceIncomingSegment
        {
            Data = new FaceIncomingSegmentData
            {
                FaceId = face.FaceId.ToString(),
                IsLarge = face.IsLargeFace,
            }
        },
        ReplyEntity reply => await ToReplyIncomingSegmentAsync(reply, type, ownerPeerUin, ct),
        ImageEntity image => new ImageIncomingSegment
        {
            Data = new ImageIncomingSegmentData
            {
                ResourceId = image.FileUuid,
                TempUrl = image.FileUrl,
                Width = (int)image.ImageSize.X,
                Height = (int)image.ImageSize.Y,
                Summary = image.Summary,
                SubType = image.SubType switch
                {
                    0 => "normal",
                    _ => "sticker",
                }
            }
        },
        RecordEntity record => new RecordIncomingSegment
        {
            Data = new RecordIncomingSegmentData
            {
                ResourceId = record.FileUuid,
                TempUrl = record.FileUrl,
                Duration = (int)record.RecordLength,
            }
        },
        VideoEntity video => new VideoIncomingSegment
        {
            Data = new VideoIncomingSegmentData
            {
                ResourceId = video.FileUuid,
                TempUrl = video.FileUrl,
                Width = (int)video.VideoSize.X,
                Height = (int)video.VideoSize.Y,
                Duration = (int)video.VideoLength,
            }
        },
        
        MarketfaceEntity marketFace => new MarketFaceIncomingSegment
        {
            Data = new MarketFaceIncomingSegmentData
            {
                EmojiPackageId = marketFace.EmojiPackageId,
                EmojiId = marketFace.EmojiId,
                Key = marketFace.Key,
                Summary = marketFace.Summary,
                Url = marketFace.EmojiId.Length >= 2
                    ? $"https://gxh.vip.qq.com/club/item/parcel/item/{marketFace.EmojiId[..2]}/{marketFace.EmojiId}/raw300.gif"
                    : string.Empty,
            }
        },
        XmlEntity xml => new XmlIncomingSegment
        {
            Data = new XmlIncomingSegmentData
            {
                ServiceId = xml.ServiceId,
                XmlPayload = xml.Xml,
            }
        },
        MarkdownEntity markdown => new MarkdownIncomingSegment
        {
            Data = new MarkdownIncomingSegmentData
            {
                Content = markdown.Data.Content,
            }
        },
        MultiMsgEntity forward => new ForwardIncomingSegment
        {
            Data = new ForwardIncomingSegmentData
            {
                ForwardId = forward.ResId ?? throw new Exception(""),
                Title = forward.Title ?? string.Empty,
                Preview = forward.Preview ?? [],
                Summary = forward.Summary ?? string.Empty,
            }
        },
        FileEntity file => new FileIncomingSegment
        {
            Data = new FileIncomingSegmentData
            {
                FileId = file.FileId,
                FileName = file.FileName,
                FileSize = file.FileSize,
                FileHash = file.FileHash,
            }
        },
        GroupFileEntity groupFile => new FileIncomingSegment
        {
            Data = new FileIncomingSegmentData
            {
                FileId = groupFile.FileId,
                FileName = groupFile.FileName,
                FileSize = groupFile.FileSize,
                FileHash = null,
            }
        },
        LightAppEntity lightApp => new LightAppIncomingSegment
        {
            Data = new LightAppIncomingSegmentData
            {
                AppName = lightApp.AppName,
                JsonPayload = lightApp.Payload,
            }
        },
        _ => null,
    };

    private async Task<ReplyIncomingSegment> ToReplyIncomingSegmentAsync(ReplyEntity reply, MessageType type, long ownerPeerUin, CancellationToken ct = default)
    {
        var message = _cache.Get(type, ownerPeerUin, reply.SrcSequence)
            ?? (type switch
            {
                MessageType.Private => await _lagrange.GetC2CMessage(
                    ownerPeerUin,
                    reply.SrcSequence,
                    reply.SrcSequence
                ).WaitAsync(ct),
                MessageType.Group => await _lagrange.GetGroupMessage(
                    ownerPeerUin,
                    reply.SrcSequence,
                    reply.SrcSequence
                ).WaitAsync(ct),
                _ => throw new NotSupportedException(),
            }).FirstOrDefault();

        
        if (message == null)
        {
            return new ReplyIncomingSegment
            {
                Data = new ReplyIncomingSegmentData
                {
                    MessageSeq = (long)reply.SrcSequence,
                    SenderId = reply.Source?.Uin ?? 0,
                    SenderName = reply.Source?.Nickname,
                    Time = 0,
                    Segments = [],
                }
            };
        }

        return new ReplyIncomingSegment
        {
            Data = new ReplyIncomingSegmentData
            {
                MessageSeq = message.Type switch
                {
                    MessageType.Private => (long)message.ClientSequence,
                    _ => (long)message.Sequence,
                },
                SenderId = message.Contact.Uin,
                SenderName = message.Contact switch
                {
                    BotFriend sender => sender.Nickname,
                    BotGroupMember member => member.Nickname,
                    BotStranger stranger => stranger.Nickname,
                    _ => throw new NotSupportedException(),
                },
                Time = message.Time,
                Segments = await ToIncomingSegmentsAsync(message.Entities, type, ownerPeerUin, ct),
            }
        };
    }

    public async Task<MessageChain> FromOutgoingSegmentsAsync(IReadOnlyList<OutgoingSegmentBase> segments, MessageType type, long ownerPeerUin, CancellationToken ct = default)
    {
        MessageChain result = [];
        foreach (var segment in segments)
        {
            result.Add(await FromOutgoingSegmentAsync(segment, type, ownerPeerUin, ct));
        }
        return result;
    }

    private async Task<IMessageEntity> FromOutgoingSegmentAsync(OutgoingSegmentBase segment, MessageType type, long ownerPeerUin, CancellationToken ct) => segment switch
    {
        TextOutgoingSegment text => new TextEntity(text.Data.Text),
        MentionOutgoingSegment mention => new MentionEntity(mention.Data.UserId, null),
        MentionAllOutgoingSegment => new MentionEntity(0, null),
        ReplyOutgoingSegment reply => await FromReplyOutgoingSegmentAsync(reply, type, ownerPeerUin, ct),
        ImageOutgoingSegment image => new ImageEntity(
            await _resourceConverter.UriToStreamAsync(image.Data.Uri, ct),
            image.Data.Summary,
            image.Data.SubType switch
            {
                "sticker" => 1,
                _ => 0,
            },
            disposeOnCompletion: true
        ),
        RecordOutgoingSegment record => new RecordEntity(
            await _resourceConverter.UriToStreamAsync(record.Data.Uri, ct),
            disposeOnCompletion: true
        ),
        FaceOutgoingSegment face => new FaceEntity(ushort.Parse(face.Data.FaceId), face.Data.IsLarge),
        VideoOutgoingSegment video => new VideoEntity(
            await _resourceConverter.UriToStreamAsync(video.Data.Uri, ct),
            video.Data.ThumbUri == null ? null : await _resourceConverter.UriToStreamAsync(video.Data.ThumbUri, ct),
            disposeOnCompletion: true
        ),
        ForwardOutgoingSegment forward => new MultiMsgEntity(
            await FromOutgoingForwardedMessagesAsync(forward.Data.Messages, ct),
            forward.Data.Title,
            forward.Data.Preview,
            forward.Data.Summary,
            forward.Data.Prompt
        ), 
        LightAppOutgoingSegment lightApp => new LightAppEntity(lightApp.Data.JsonPayload),
        _ => throw new NotSupportedException(),
    };

    private async Task<ReplyEntity> FromReplyOutgoingSegmentAsync(ReplyOutgoingSegment reply, MessageType type, long ownerPeerUin, CancellationToken ct)
    {
        var message = _cache.Get(type, ownerPeerUin, (ulong)reply.Data.MessageSeq)
            ?? (type switch
            {
                MessageType.Private => await _lagrange.GetC2CMessage(
                    ownerPeerUin,
                    (ulong)reply.Data.MessageSeq,
                    (ulong)reply.Data.MessageSeq
                ).WaitAsync(ct),
                MessageType.Group => await _lagrange.GetGroupMessage(
                    ownerPeerUin,
                    (ulong)reply.Data.MessageSeq,
                    (ulong)reply.Data.MessageSeq
                ).WaitAsync(ct),
                _ => throw new NotSupportedException(),
            }).First();

        return new ReplyEntity(message);
    }

    private async Task<List<BotMessage>> FromOutgoingForwardedMessagesAsync(IReadOnlyList<OutgoingForwardedMessage> messages, CancellationToken ct)
    {
        List<BotMessage> result = [];
        foreach (var message in messages)
        {
            result.Add(BotMessage.CreateCustomFriend(
                message.UserId,
                message.SenderName,
                0,
                string.Empty,
                message.Time ?? DateTimeOffset.Now.ToUnixTimeSeconds(),
                await FromForwardOutgoingSegmentsAsync(message.Segments, ct)
            ));
        }
        return result;
    }

    private async Task<MessageChain> FromForwardOutgoingSegmentsAsync(IReadOnlyList<OutgoingSegmentBase> segments, CancellationToken ct)
    {
        MessageChain chain = [];
        foreach (var segment in segments)
        {
            chain.Add(await FromForwardOutgoingSegmentAsync(segment, ct));
        }
        return chain;
    }

    private async Task<IMessageEntity> FromForwardOutgoingSegmentAsync(OutgoingSegmentBase segment, CancellationToken ct) => segment switch
    {
        TextOutgoingSegment text => new TextEntity(text.Data.Text),
        MentionOutgoingSegment mention => new MentionEntity(mention.Data.UserId, null),
        MentionAllOutgoingSegment => new MentionEntity(0, null),
        ReplyOutgoingSegment => new ReplyEntity(), 
        ImageOutgoingSegment image => new ImageEntity(
            await _resourceConverter.UriToStreamAsync(image.Data.Uri, ct),
            image.Data.Summary,
            image.Data.SubType switch
            {
                "sticker" => 1,
                _ => 0,
            },
            disposeOnCompletion: true
        ), 
        RecordOutgoingSegment record => new RecordEntity(
            await _resourceConverter.UriToStreamAsync(record.Data.Uri, ct),
            disposeOnCompletion: true
        ), 
        FaceOutgoingSegment face => new FaceEntity(ushort.Parse(face.Data.FaceId), face.Data.IsLarge),
        VideoOutgoingSegment video => new VideoEntity(
            await _resourceConverter.UriToStreamAsync(video.Data.Uri, ct),
            video.Data.ThumbUri == null ? null : await _resourceConverter.UriToStreamAsync(video.Data.ThumbUri, ct),
            disposeOnCompletion: true
        ), 
        ForwardOutgoingSegment forward => new MultiMsgEntity(
            await FromOutgoingForwardedMessagesAsync(forward.Data.Messages, ct),
            forward.Data.Title,
            forward.Data.Preview,
            forward.Data.Summary,
            forward.Data.Prompt
        ), 
        LightAppOutgoingSegment lightApp => new LightAppEntity(lightApp.Data.JsonPayload),
        _ => throw new NotSupportedException(),
    };
}