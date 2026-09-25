namespace Lagrange.Core.Events.EventArgs;

public class BotGroupNameChangeEvent(long groupUin, string oldName, string newName, long operatorUin) : EventBase
{
    public long GroupUin { get; } = groupUin;

    public string OldName { get; } = oldName;

    public string NewName { get; } = newName;

    public long OperatorUin { get; } = operatorUin;

    public override string ToEventMessage()
    {
        return $"{nameof(BotGroupNameChangeEvent)}: Group={GroupUin}, {OldName}->{NewName} by {OperatorUin}";
    }
}
