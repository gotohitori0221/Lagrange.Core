using System.Runtime.InteropServices;
using Lagrange.Core.Events.EventArgs;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotQrCodeQueryEventStruct : IEventStruct
    {
        public BotQrCodeQueryEventStruct() { }

        public byte State = 0;

        public static implicit operator BotQrCodeQueryEvent(BotQrCodeQueryEventStruct e)
        {
            return new BotQrCodeQueryEvent((BotQrCodeQueryEvent.TransEmpState)e.State);
        }

        public static implicit operator BotQrCodeQueryEventStruct(BotQrCodeQueryEvent e)
        {
            return new BotQrCodeQueryEventStruct { State = (byte)e.State };
        }
    }
}
