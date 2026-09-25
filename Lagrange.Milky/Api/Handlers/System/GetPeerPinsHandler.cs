using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;
using Lagrange.Milky.Converters;
using Lagrange.Milky.Models;

namespace Lagrange.Milky.Api.Handlers.System;

[ApiHandler("get_peer_pins")]
public sealed class GetPeerPinsHandler(BotContext lagrange, MilkyConverter converter) : INoRequestApiHandler<GetPeerPinsHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;
    private readonly MilkyConverter _converter = converter;

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(CancellationToken ct)
    {
        var (friendUids, groupUins) = await _lagrange.FetchPins().WaitAsync(ct);

        var friends = new List<Models.Friend>();
        if (friendUids.Count > 0)
        {
            var allFriends = await _lagrange.FetchFriends().WaitAsync(ct);
            friends.AddRange(allFriends.Where(f => friendUids.Contains(f.Uid)).Select(_converter.ToFriend));
        }

        var groups = new List<Models.Group>();
        if (groupUins.Count > 0)
        {
            var allGroups = await _lagrange.FetchGroups().WaitAsync(ct);
            groups.AddRange(allGroups.Where(g => groupUins.Contains((uint)g.Uin)).Select(_converter.ToGroup));
        }

        return new MilkyApiResponse<Result>(new Result
        {
            Friends = friends,
            Groups = groups,
        });
    }

    public sealed class Result
    {
        [JsonPropertyName("friends")] public required List<Models.Friend> Friends { get; init; }
        [JsonPropertyName("groups")] public required List<Models.Group> Groups { get; init; }
    }
}
