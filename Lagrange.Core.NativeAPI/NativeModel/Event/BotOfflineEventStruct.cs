using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotOfflineEventStruct : IEventStruct
    {
        public BotOfflineEventStruct() { }

        public int Reason = 0;

        ByteArrayNative Tag = new();

        ByteArrayNative Message = new();

        public static implicit operator BotOfflineEvent(BotOfflineEventStruct e)
        {
            return new BotOfflineEvent((BotOfflineEvent.Reasons)e.Reason, (
             Encoding.UTF8.GetString(e.Tag), Encoding.UTF8.GetString(e.Message)
            ));
        }

        public static implicit operator BotOfflineEventStruct(BotOfflineEvent e)
        {
            return new BotOfflineEventStruct()
            {
                Reason = (int)e.Reason,
                Tag = Encoding.UTF8.GetBytes(e.Tips?.Tag ?? ""),
                Message = Encoding.UTF8.GetBytes(e.Tips?.Message ?? ""),
            };
        }
    }
}