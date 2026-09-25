using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotLoginEventStruct : IEventStruct
    {
        public BotLoginEventStruct() { }
        public int State = 0;
        public ByteArrayNative Tag = new();
        public ByteArrayNative Message = new();

        public static implicit operator BotLoginEvent(BotLoginEventStruct e)
        {
            return new BotLoginEvent(
                e.State,
                (Encoding.UTF8.GetString(e.Tag), Encoding.UTF8.GetString(e.Message))
            );
        }
        
        public static implicit operator BotLoginEventStruct(BotLoginEvent e)
        {
            return new BotLoginEventStruct()
            {
                State = e.State,
                Tag = Encoding.UTF8.GetBytes(e.Error?.Tag ?? string.Empty),
                Message = Encoding.UTF8.GetBytes(e.Error?.Message ?? string.Empty)
            };
        }
    }
}