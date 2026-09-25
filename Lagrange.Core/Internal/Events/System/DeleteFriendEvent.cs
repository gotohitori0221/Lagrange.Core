using Lagrange.Core.Events;

namespace Lagrange.Core.Internal.Events.System;

internal class DeleteFriendEventReq(string targetUid, bool block) : ProtocolEvent
{
    public string TargetUid { get; } = targetUid;

    public bool Block { get; } = block;
}

internal class DeleteFriendEventResp(int resultCode) : ProtocolEvent
{
    public int ResultCode { get; } = resultCode;
}
