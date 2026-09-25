using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotNewDeviceVerifyEventStruct : IEventStruct
    {
        public BotNewDeviceVerifyEventStruct() { }
        
        public ByteArrayNative Url = new();
        
        public static implicit operator BotNewDeviceVerifyEvent(BotNewDeviceVerifyEventStruct e)
        {
            return new BotNewDeviceVerifyEvent(Encoding.UTF8.GetString(e.Url));
        }
        
        public static implicit operator BotNewDeviceVerifyEventStruct(BotNewDeviceVerifyEvent e)
        {
            return new BotNewDeviceVerifyEventStruct() { Url = Encoding.UTF8.GetBytes(e.Url) };
        }
    }
}