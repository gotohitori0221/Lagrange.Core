using Lagrange.Core.Events;

namespace Lagrange.Core.Internal.Events.System;

internal class SetFriendRequestEventReq(string targetUid, bool accept) : ProtocolEvent
{
    public string TargetUid { get; } = targetUid;

    public bool Accept { get; } = accept;
}

internal class SetFriendRequestEventResp(int resultCode) : ProtocolEvent
{
    public int ResultCode { get; } = resultCode;
}
