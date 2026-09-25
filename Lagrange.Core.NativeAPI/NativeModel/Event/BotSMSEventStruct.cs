using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotSMSEventStruct : IEventStruct
    {
        public BotSMSEventStruct() { }

        public ByteArrayNative Url = new();

        public ByteArrayNative Phone = new();

        public static implicit operator BotSMSEvent(BotSMSEventStruct e)
        {
            return new BotSMSEvent(
                Encoding.UTF8.GetString(e.Url),
                Encoding.UTF8.GetString(e.Phone)
            );
        }

        public static implicit operator BotSMSEventStruct(BotSMSEvent e)
        {
            return new BotSMSEventStruct()
            {
                Url = Encoding.UTF8.GetBytes(e.Url ?? string.Empty),
                Phone = Encoding.UTF8.GetBytes(e.Phone)
            };
        }
    }
}
