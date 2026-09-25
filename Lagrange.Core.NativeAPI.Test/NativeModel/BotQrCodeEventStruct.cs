using System.Runtime.InteropServices;

namespace Lagrange.Core.NativeAPI.Test.NativeModel
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotQrCodeEventStruct
    {
        public BotQrCodeEventStruct() { }

        public ByteArrayNative Url = new();

        public ByteArrayNative Image = new();
    }
}
