using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Common.Entity;

namespace Lagrange.Core.NativeAPI.NativeModel.Common;
[StructLayout(LayoutKind.Sequential)]
public class BotGroupExitNotificationStruct(BotGroupExitNotification e) : BotGroupNotificationBaseStruct(e)
{
    public static implicit operator BotGroupExitNotificationStruct(BotGroupExitNotification e)
    {
        return new BotGroupExitNotificationStruct(e);
    }
}
