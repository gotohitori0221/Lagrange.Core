using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Event
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotFriendRecallEventStruct : IEventStruct
    {
        public BotFriendRecallEventStruct() { }

        public long PeerUin = 0;

        public long AuthorUin = 0;

        public ulong Sequence = 0;

        public ByteArrayNative Tip = new();

        public static implicit operator BotFriendRecallEventStruct(BotFriendRecallEvent e)
        {
            return new BotFriendRecallEventStruct()
            {
                PeerUin = e.PeerUin,
                AuthorUin = e.AuthorUin,
                Sequence = e.Sequence,
                Tip = Encoding.UTF8.GetBytes(e.Tip)
            };
        }
    }
}
