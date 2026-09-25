namespace Lagrange.Core.Common.Entity;

public class BotGroupSetAdminNotification(long group, ulong sequence, long targetUin, string targetUid, long operatorUin, string operatorUid) : BotGroupNotificationBase(group, sequence, BotGroupNotificationType.SetAdmin, targetUin, targetUid)
{
    public long OperatorUin { get; } = operatorUin;

    public string OperatorUid { get; } = operatorUid;
}
