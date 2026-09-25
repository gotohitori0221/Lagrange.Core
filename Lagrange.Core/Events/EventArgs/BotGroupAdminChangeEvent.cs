namespace Lagrange.Core.Events.EventArgs;

public class BotGroupAdminChangeEvent(long groupUin, long targetUin, bool isAdmin, long operatorUin) : EventBase
{
    public long GroupUin { get; } = groupUin;

    public long TargetUin { get; } = targetUin;

    public bool IsAdmin { get; } = isAdmin;

    public long OperatorUin { get; } = operatorUin;

    public override string ToEventMessage() =>
        $"{nameof(BotGroupAdminChangeEvent)}: Group={GroupUin}, Target={TargetUin}, IsAdmin={IsAdmin}, Operator={OperatorUin}";
}
