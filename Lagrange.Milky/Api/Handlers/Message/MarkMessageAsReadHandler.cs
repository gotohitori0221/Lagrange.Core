using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.Message;

[ApiHandler("mark_message_as_read")]
public sealed class MarkMessageAsReadHandler(BotContext lagrange) : INoResultApiHandler<MarkMessageAsReadHandler.Request>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse> HandleAsync(Request request, CancellationToken ct)
    {
        if (request.MessageScene == "group")
        {
            await _lagrange.MarkMessageAsRead(request.PeerId, (uint)request.MessageSeq).WaitAsync(ct);
        }
        else
        {
            var friends = await _lagrange.FetchFriends().WaitAsync(ct);
            var friend = friends.FirstOrDefault(f => f.Uin == request.PeerId);
            if (friend == null) return new MilkyApiResponse(-404, "friend not found");

            await _lagrange.MarkC2CMessageAsRead(friend.Uid, (uint)request.MessageSeq, 0).WaitAsync(ct);
        }
        return new MilkyApiResponse();
    }

    public sealed class Request
    {
        [JsonPropertyName("message_scene")] public required string MessageScene { get; init; }
        [JsonPropertyName("peer_id")] public required long PeerId { get; init; }
        [JsonPropertyName("message_seq")] public required long MessageSeq { get; init; }
    }
}
