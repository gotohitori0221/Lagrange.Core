using Lagrange.Core.Events;

namespace Lagrange.Core.Internal.Events.System;

internal class GroupSetAdminEventReq(uint groupUin, string uid, bool isAdmin) : ProtocolEvent
{
    public uint GroupUin { get; } = groupUin;

    public string Uid { get; } = uid;

    public bool IsAdmin { get; } = isAdmin;
}

internal class GroupSetAdminEventResp(int resultCode) : ProtocolEvent
{
    public int ResultCode { get; } = resultCode;
}
