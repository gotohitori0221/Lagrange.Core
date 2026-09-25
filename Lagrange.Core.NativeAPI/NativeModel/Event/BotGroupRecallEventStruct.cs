using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotGroupRecallEventStruct : IEventStruct
    {
        public BotGroupRecallEventStruct() { }

        public long GroupUin = 0;

        public ulong Sequence = 0;

        public long AuthorUin = 0;

        public long OperatorUin = 0;

        public ByteArrayNative Tip = new();

        public static implicit operator BotGroupRecallEventStruct(BotGroupRecallEvent e)
        {
            return new BotGroupRecallEventStruct()
            {
                GroupUin = e.GroupUin,
                Sequence = e.Sequence,
                AuthorUin = e.AuthorUin,
                OperatorUin = e.OperatorUin,
                Tip = Encoding.UTF8.GetBytes(e.Tip)
            };
        }
    }
}