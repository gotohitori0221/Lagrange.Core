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

namespace Lagrange.Milky.Api.Handlers.Message;

[ApiHandler("get_forwarded_messages")]
public sealed class GetForwardedMessagesHandler(BotContext lagrange, MilkyConverter converter) : IApiHandler<GetForwardedMessagesHandler.Request, GetForwardedMessagesHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;
    private readonly MilkyConverter _converter = converter;

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(Request request, CancellationToken ct)
    {
        List<BotMessage> messages;
        try
        {
            messages = await _lagrange.GetForwardedMessages(request.ForwardId, true).WaitAsync(ct);
        }
        catch
        {
            messages = await _lagrange.GetForwardedMessages(request.ForwardId, false).WaitAsync(ct);
        }

        var forwarded = new IncomingForwardedMessage[messages.Count];
        for (int i = 0; i < messages.Count; i++)
            forwarded[i] = await _converter.ToIncomingForwardedMessageAsync(messages[i], ct);

        return new MilkyApiResponse<Result>(new Result { Messages = forwarded });
    }

    public sealed class Request
    {
        [JsonPropertyName("forward_id")] public required string ForwardId { get; init; }
    }

    public sealed class Result
    {
        [JsonPropertyName("messages")] public required IReadOnlyList<IncomingForwardedMessage> Messages { get; init; }
    }
}
