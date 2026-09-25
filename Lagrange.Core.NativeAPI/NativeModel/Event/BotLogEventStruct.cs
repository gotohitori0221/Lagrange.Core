using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotLogEventStruct : IEventStruct
    {
        public BotLogEventStruct() { }

        public int Level = 0;
        public ByteArrayNative Tag = new();
        public ByteArrayNative Message = new();

        public static implicit operator BotLogEventStruct(BotLogEvent e)
        {
            return new BotLogEventStruct()
            {
                Level = (int)e.Level,
                Tag = Encoding.UTF8.GetBytes(e.Tag),
                Message = Encoding.UTF8.GetBytes(e.Message)
            };
        }
        
        public static implicit operator BotLogEvent(BotLogEventStruct e)
        {
            return new BotLogEvent(
                Encoding.UTF8.GetString(e.Tag),
                (LogLevel)e.Level,
                Encoding.UTF8.GetString(e.Message),
                null 
            );
        }
    }
}
