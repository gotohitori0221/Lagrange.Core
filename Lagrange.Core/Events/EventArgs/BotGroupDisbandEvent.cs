namespace Lagrange.Core.Events.EventArgs;

public class BotGroupDisbandEvent(long groupUin, long? operatorUin) : EventBase
{
    public long GroupUin { get; } = groupUin;

    public long? OperatorUin { get; } = operatorUin;

    public override string ToEventMessage()
    {
        return OperatorUin == null
            ? $"{nameof(BotGroupDisbandEvent)}: GroupUin={GroupUin}"
            : $"{nameof(BotGroupDisbandEvent)}: GroupUin={GroupUin}, OperatorUin={OperatorUin}";
    }
}
