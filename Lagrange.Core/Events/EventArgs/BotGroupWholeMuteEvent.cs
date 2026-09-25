namespace Lagrange.Core.Events.EventArgs;

public class BotGroupWholeMuteEvent(long groupUin, long operatorUin, bool isMuted) : EventBase
{
    public long GroupUin { get; } = groupUin;

    public long OperatorUin { get; } = operatorUin;

    public bool IsMuted { get; } = isMuted;

    public override string ToEventMessage()
    {
        return $"{nameof(BotGroupWholeMuteEvent)}: Group={GroupUin}, IsMuted={IsMuted} by {OperatorUin}";
    }
}
