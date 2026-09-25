namespace Lagrange.Core.Events.EventArgs;

public class BotPinChangedEvent(long uin, long? groupUin, bool isPin) : EventBase
{
    public long Uin { get; } = uin;

    public long? GroupUin { get; } = groupUin;

    public bool IsPin { get; } = isPin;

    public PinChangedChatType ChatType => GroupUin.HasValue ? PinChangedChatType.Group : PinChangedChatType.Friend;

    public override string ToEventMessage() =>
        $"{nameof(BotPinChangedEvent)} {{ChatType: {ChatType} | Uin: {Uin} | GroupUin: {GroupUin} | IsPin: {IsPin}}}";

    public enum PinChangedChatType
    {
        Friend,
        Group,
        Service
    }
}
