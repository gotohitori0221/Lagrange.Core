using Lagrange.Core.Common.Entity;
using Lagrange.Core.Events;

namespace Lagrange.Core.Internal.Events.System;

internal class FetchFriendsEventReq(byte[]? cookie) : ProtocolEvent
{
    public byte[]? Cookie { get; set; } = cookie; 
}

internal class FetchFriendsEventResp(List<BotFriend> friends, List<BotFriendCategory> category, byte[]? cookie) : ProtocolEvent
{
    public List<BotFriend> Friends { get; } = friends;

    public List<BotFriendCategory> Category { get; } = category;
    
    public byte[]? Cookie { get; } = cookie;
}