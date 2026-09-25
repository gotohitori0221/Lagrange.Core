using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.Group;

[ApiHandler("set_group_member_admin")]
public sealed class SetGroupMemberAdminHandler(BotContext lagrange) : INoResultApiHandler<SetGroupMemberAdminHandler.Request>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse> HandleAsync(Request request, CancellationToken ct)
    {
        var members = await _lagrange.FetchMembers(request.GroupId, true).WaitAsync(ct);
        var member = members.FirstOrDefault(m => m.Uin == request.UserId);
        if (member == null) return new MilkyApiResponse(-404, "member not found");

        await _lagrange.GroupSetAdmin(request.GroupId, member.Uid, request.IsSet).WaitAsync(ct);
        return new MilkyApiResponse();
    }

    public sealed class Request
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
        [JsonPropertyName("is_set")] public bool IsSet { get; init; } = true;
    }
}
