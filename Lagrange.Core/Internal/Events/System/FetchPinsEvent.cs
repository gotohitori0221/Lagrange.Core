using Lagrange.Core.Events;

namespace Lagrange.Core.Internal.Events.System;

internal class FetchPinsEventReq : ProtocolEvent
{
    public static readonly FetchPinsEventReq Instance = new();
}

internal class FetchPinsEventResp(List<string> friendUids, List<uint> groupUins) : ProtocolEvent
{
    public List<string> FriendUids { get; } = friendUids;

    public List<uint> GroupUins { get; } = groupUins;
}
