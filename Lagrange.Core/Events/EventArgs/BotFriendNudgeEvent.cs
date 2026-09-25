namespace Lagrange.Core.Events.EventArgs;

public class BotFriendNudgeEvent(long peerUin, long operatorUin, bool isSelfSend, bool isSelfReceive, string action, string actionImgUrl, string suffix) : EventBase
{
    public long PeerUin { get; } = peerUin;

    public long OperatorUin { get; } = operatorUin;

    public bool IsSelfSend { get; } = isSelfSend;

    public bool IsSelfReceive { get; } = isSelfReceive;

    public string Action { get; } = action;

    public string ActionImageUrl { get; } = actionImgUrl;

    public string Suffix { get; } = suffix;

    public override string ToEventMessage() =>
        $"{nameof(BotFriendNudgeEvent)}: {OperatorUin} {Action}({ActionImageUrl}) {PeerUin} {Suffix}";
}
