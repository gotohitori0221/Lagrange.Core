using Lagrange.Core.Events;

namespace Lagrange.Core.Internal.Events.Message;

internal class MarkReadedEventReq : ProtocolEvent
{
    public uint? GroupUin { get; init; }

    public string? TargetUid { get; init; }

    public uint StartSequence { get; init; }

    public uint Time { get; init; }
}

internal class MarkReadedEventResp(int resultCode) : ProtocolEvent
{
    public int ResultCode { get; } = resultCode;
}
