using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;
using Lagrange.Milky.Converters;

namespace Lagrange.Milky.Api.Handlers.Friend;

[ApiHandler("get_friend_requests")]
public sealed class GetFriendRequestsHandler(BotContext lagrange, MilkyConverter converter) : IApiHandler<GetFriendRequestsHandler.Request, GetFriendRequestsHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;
    private readonly MilkyConverter _converter = converter;

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(Request request, CancellationToken ct)
    {
        var requests = await _lagrange.FetchFriendRequests().WaitAsync(ct);
        return new MilkyApiResponse<Result>(new Result
        {
            Requests = [.. requests.Take(request.Limit).Select(_converter.ToFriendRequest)]
        });
    }

    public sealed class Request
    {
        [JsonPropertyName("limit")] public int Limit { get; init; } = 20;
        [JsonPropertyName("is_filtered")] public bool IsFiltered { get; init; }
    }

    public sealed class Result
    {
        [JsonPropertyName("requests")]
        public required IReadOnlyList<Models.FriendRequest> Requests { get; init; }
    }
}
