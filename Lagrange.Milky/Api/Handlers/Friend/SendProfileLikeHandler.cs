using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.Friend;

[ApiHandler("send_profile_like")]
public sealed class SendProfileLikeHandler(BotContext lagrange) : INoResultApiHandler<SendProfileLikeHandler.Request>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse> HandleAsync(Request request, CancellationToken ct)
    {
        string? uid = null;
        var friends = await _lagrange.FetchFriends().WaitAsync(ct);
        var friend = friends.FirstOrDefault(f => f.Uin == request.UserId);
        if (friend != null)
        {
            uid = friend.Uid;
        }
        else
        {
            try
            {
                var stranger = await _lagrange.FetchStranger(request.UserId).WaitAsync(ct);
                if (!string.IsNullOrEmpty(stranger.Uid)) uid = stranger.Uid;
            }
            catch { }
        }

        if (string.IsNullOrEmpty(uid)) return new MilkyApiResponse(-404, "User not found");

        await _lagrange.SendProfileLike(uid, (uint)request.Count).WaitAsync(ct);
        return new MilkyApiResponse();
    }

    public sealed class Request
    {
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
        [JsonPropertyName("count")] public int Count { get; init; } = 1;
    }
}
