using System.Runtime.InteropServices;

namespace Lagrange.Core.NativeAPI.Test.NativeModel
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotLogEventStruct
    {
        public BotLogEventStruct() { }

        public int Level = 0;
        public ByteArrayNative Tag = new();
        public ByteArrayNative Message = new();
    }
}
