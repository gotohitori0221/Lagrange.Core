using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.Friend;

[ApiHandler("delete_friend")]
public sealed class DeleteFriendHandler(BotContext lagrange) : INoResultApiHandler<DeleteFriendHandler.Request>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse> HandleAsync(Request request, CancellationToken ct)
    {
        var friends = await _lagrange.FetchFriends().WaitAsync(ct);
        var friend = friends.FirstOrDefault(f => f.Uin == request.UserId);
        if (friend == null) return new MilkyApiResponse(-404, "friend not found");

        await _lagrange.DeleteFriend(friend.Uid, false).WaitAsync(ct);
        return new MilkyApiResponse();
    }

    public sealed class Request
    {
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
    }
}
