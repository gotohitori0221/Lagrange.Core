using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.Friend;

[ApiHandler("reject_friend_request")]
public sealed class RejectFriendRequestHandler(BotContext lagrange) : INoResultApiHandler<RejectFriendRequestHandler.Request>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse> HandleAsync(Request request, CancellationToken ct)
    {
        return await _lagrange.SetFriendRequest(request.InitiatorUid, false).WaitAsync(ct)
            ? new MilkyApiResponse()
            : new MilkyApiResponse(-500, "unknown error");
    }

    public sealed class Request
    {
        [JsonPropertyName("initiator_uid")] public required string InitiatorUid { get; init; }
        [JsonPropertyName("is_filtered")] public bool IsFiltered { get; init; }
        [JsonPropertyName("reason")] public string? Reason { get; init; }
    }
}
