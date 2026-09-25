using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.Friend;

[ApiHandler("accept_friend_request")]
public sealed class AcceptFriendRequestHandler(BotContext lagrange) : INoResultApiHandler<AcceptFriendRequestHandler.Request>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse> HandleAsync(Request request, CancellationToken ct)
    {
        return await _lagrange.SetFriendRequest(request.InitiatorUid, true).WaitAsync(ct)
            ? new MilkyApiResponse()
            : new MilkyApiResponse(-500, "unknown error");
    }

    public sealed class Request
    {
        [JsonPropertyName("initiator_uid")] public required string InitiatorUid { get; init; }
        [JsonPropertyName("is_filtered")] public bool IsFiltered { get; init; }
    }
}
