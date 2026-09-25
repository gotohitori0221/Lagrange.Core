using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message.Entity;

[StructLayout(LayoutKind.Sequential)]
public struct MultiMsgEntityStruct
{
    public MultiMsgEntityStruct() { }
    
    public IntPtr Messages;
    
    public int MessageCount;
    
    public ByteArrayNative ResId;
    
    public static implicit operator MultiMsgEntityStruct(Lagrange.Core.Message.Entities.MultiMsgEntity entity)
    {
        var messagesStructs = entity.Messages
            .Select(m => (BotMessageStruct)m)
            .ToArray();
        
        IntPtr messagesPtr = Marshal.AllocHGlobal(Marshal.SizeOf<BotMessageStruct>() * messagesStructs.Length);
        
        for (int i = 0; i < messagesStructs.Length; i++)
        {
            Marshal.StructureToPtr(messagesStructs[i], messagesPtr + i * Marshal.SizeOf<BotMessageStruct>(), false);
        }
        
        return new MultiMsgEntityStruct
        {
            Messages = messagesPtr,
            MessageCount = messagesStructs.Length,
            ResId = entity.ResId != null 
                ? Encoding.UTF8.GetBytes(entity.ResId) 
                : []
        };
    }
}