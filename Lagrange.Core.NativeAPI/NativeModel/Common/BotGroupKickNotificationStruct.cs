using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Common.Entity;

namespace Lagrange.Core.NativeAPI.NativeModel.Common;
[StructLayout(LayoutKind.Sequential)]
public class BotGroupKickNotificationStruct(BotGroupKickNotification notification) : BotGroupNotificationBaseStruct(notification)
{
    public long OperatorUin = notification.OperatorUin;

    public ByteArrayNative OperatorUid { get; } = Encoding.UTF8.GetBytes(notification.OperatorUid);

    public static implicit operator BotGroupKickNotificationStruct(BotGroupKickNotification e)
    {
        return new BotGroupKickNotificationStruct(e);
    }
}
