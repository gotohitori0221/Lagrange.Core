using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Message;
using Lagrange.Milky.Api.Attributes;
using Lagrange.Milky.Converters;
using Lagrange.Milky.Models;
using Lagrange.Milky.Models.Messages;
using Lagrange.Milky.Storage;

namespace Lagrange.Milky.Api.Handlers.Message;

[ApiHandler("get_history_messages")]
public sealed class GetHistoryMessagesHandler(BotContext lagrange, MessageStore store, MilkyConverter converter) : IApiHandler<GetHistoryMessagesHandler.Request, GetHistoryMessagesHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;
    private readonly MessageStore _store = store;
    private readonly MilkyConverter _converter = converter;

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(Request request, CancellationToken ct)
    {
        var messageType = request.MessageScene switch
        {
            "friend" => MessageType.Private,
            "group" => MessageType.Group,
            _ => throw new NotSupportedException(),
        };

        long endSequence;
        if (request.StartMessageSeq.HasValue)
        {
            endSequence = request.StartMessageSeq.Value;
        }
        else if (request.MessageScene == "group")
        {
            endSequence = (await _lagrange.FetchGroupExtra(request.PeerId).WaitAsync(ct)).LatestMessageSequence;
        }
        else if (request.MessageScene == "friend")
        {
            var latest = _store.GetLatestSequence(MessageType.Private, request.PeerId);
            endSequence = (long)(latest ?? 0);
        }
        else
        {
            throw new NotSupportedException();
        }

        ulong startSeq = endSequence == 0 ? 0 : (ulong)Math.Max(0, endSequence - request.Limit);
        ulong endSeq = endSequence == 0 ? (ulong)request.Limit : (ulong)endSequence;

        
        var dbMessages = _store.GetRange(messageType, request.PeerId, startSeq, endSeq, request.Limit);
        IReadOnlyList<BotMessage> messages;
        if (dbMessages.Count >= Math.Min(request.Limit, (int)(endSeq - startSeq)))
        {
            messages = dbMessages;
        }
        else
        {
            messages = request.MessageScene switch
            {
                "friend" => await _lagrange.GetC2CMessage(request.PeerId, startSeq, endSeq).WaitAsync(ct),
                "group" => await _lagrange.GetGroupMessage(request.PeerId, startSeq, endSeq).WaitAsync(ct),
                _ => throw new NotSupportedException(),
            };
        }

        var incomingMessages = new IncomingMessageBase[messages.Count];
        for (int i = 0; i < messages.Count; i++)
            incomingMessages[i] = await _converter.ToIncomingMessageAsync(messages[i], ct);

        long? nextSequence = incomingMessages.Length == 0
            ? null
            : incomingMessages.Min(m => m.MessageSeq) - 1;
        if (nextSequence.HasValue && nextSequence.Value < 0) nextSequence = null;

        return new MilkyApiResponse<Result>(new Result
        {
            Messages = [.. incomingMessages.OrderBy(m => m.MessageSeq)],
            NextMessageSeq = nextSequence,
        });
    }

    public sealed class Request(string messageScene, long peerId, long? startMessageSeq = null, int limit = 20)
    {
        [JsonPropertyName("message_scene")] public required string MessageScene { get; init; } = messageScene;
        [JsonPropertyName("peer_id")] public required long PeerId { get; init; } = peerId;
        [JsonPropertyName("start_message_seq")] public long? StartMessageSeq { get; init; } = startMessageSeq;
        [JsonPropertyName("limit")] public int Limit { get; init; } = limit;
    }

    public sealed class Result
    {
        [JsonPropertyName("messages")] public required IReadOnlyList<IncomingMessageBase> Messages { get; init; }
        [JsonPropertyName("next_message_seq")] public required long? NextMessageSeq { get; init; }
    }
}
