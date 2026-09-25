using System.Runtime.InteropServices;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;
using Lagrange.Core.NativeAPI.NativeModel.Message;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotMessageEventStruct : IEventStruct
    {
        public BotMessageEventStruct() { }
        
        public BotMessageStruct Message = new();

        
        
        
        
        
        
        
        
        
        public static implicit operator BotMessageEventStruct(BotMessageEvent e)
        {
            return new BotMessageEventStruct() { Message = e.Message };
        }
    }
}