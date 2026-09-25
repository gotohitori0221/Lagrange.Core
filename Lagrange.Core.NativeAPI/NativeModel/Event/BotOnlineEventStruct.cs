using System.Runtime.InteropServices;
using Lagrange.Core.Events.EventArgs;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotOnlineEventStruct : IEventStruct
    {
        public BotOnlineEventStruct() { }
        
        public int Reason = 0;

        public static implicit operator BotOnlineEvent(BotOnlineEventStruct e)
        {
            return new BotOnlineEvent((BotOnlineEvent.Reasons)e.Reason);
        }
        
        public static implicit operator BotOnlineEventStruct(BotOnlineEvent e)
        {
            return new BotOnlineEventStruct() { Reason = (int)e.Reason };
        }
    }
}