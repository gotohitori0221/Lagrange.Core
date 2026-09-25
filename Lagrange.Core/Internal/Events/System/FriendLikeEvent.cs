using Lagrange.Core.Events;

namespace Lagrange.Core.Internal.Events.System;

internal class FriendLikeEventReq(string targetUid, uint count) : ProtocolEvent
{
    public string TargetUid { get; } = targetUid;

    public uint Count { get; } = count;
}

internal class FriendLikeEventResp(int resultCode) : ProtocolEvent
{
    public int ResultCode { get; } = resultCode;
}
