namespace Lagrange.Core.Events.EventArgs;

public class BotGroupMuteEvent(long groupUin, long operatorUin, long targetUin, uint duration) : EventBase
{
    public long GroupUin { get; } = groupUin;

    public long OperatorUin { get; } = operatorUin;

    public long TargetUin { get; } = targetUin;

    
    public uint Duration { get; } = duration;

    public override string ToEventMessage()
    {
        return $"{nameof(BotGroupMuteEvent)}: Group={GroupUin}, Target={TargetUin}, Duration={Duration}s by {OperatorUin}";
    }
}
