using System.Runtime.InteropServices;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotRefreshKeystoreEventStruct : IEventStruct
    {
        public BotRefreshKeystoreEventStruct() { }

        public BotKeystoreStruct Keystore = new();

        public static implicit operator BotRefreshKeystoreEvent(BotRefreshKeystoreEventStruct e)
        {
            return new BotRefreshKeystoreEvent(e.Keystore);
        }

        public static implicit operator BotRefreshKeystoreEventStruct(BotRefreshKeystoreEvent e)
        {
            return new BotRefreshKeystoreEventStruct() { Keystore = e.Keystore };
        }
    }
}
