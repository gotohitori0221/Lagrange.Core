using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotQrCodeEventStruct : IEventStruct
    {
        public BotQrCodeEventStruct() { }

        public ByteArrayNative Url = new();

        public ByteArrayNative Image = new();

        public static implicit operator BotQrCodeEvent(BotQrCodeEventStruct e)
        {
            return new BotQrCodeEvent(Encoding.UTF8.GetString(e.Url), e.Image);
        }

        public static implicit operator BotQrCodeEventStruct(BotQrCodeEvent e)
        {
            return new BotQrCodeEventStruct()
            {
                Url = Encoding.UTF8.GetBytes(e.Url),
                Image = e.Image
            };
        }
    }
}
