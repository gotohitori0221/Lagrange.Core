using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotFriendStruct
    {
        public BotFriendStruct() { }

        public long Uin = 0;

        public ByteArrayNative Nickname = new();

        public ByteArrayNative Uid = new();

        public int Age = 0;

        public int Gender = 0;

        public ByteArrayNative Remarks = new();

        public ByteArrayNative PersonalSign = new();

        public ByteArrayNative Qid = new();

        public BotFriendCategoryStruct Category = new();

        public static implicit operator BotFriend(BotFriendStruct friend)
        {
            return new BotFriend(
                friend.Uin,
                Encoding.UTF8.GetString(friend.Nickname),
                Encoding.UTF8.GetString(friend.Uid),
                Encoding.UTF8.GetString(friend.Remarks),
                Encoding.UTF8.GetString(friend.PersonalSign),
                Encoding.UTF8.GetString(friend.Qid),
                friend.Category
            );
        }
        
        public static implicit operator BotFriendStruct(BotFriend friend)
        {
            return new BotFriendStruct()
            {
                Uin = friend.Uin,
                Nickname = Encoding.UTF8.GetBytes(friend.Nickname),
                Uid = Encoding.UTF8.GetBytes(friend.Uid),
                Age = friend.Age,
                Gender = (int)friend.Gender,
                Remarks = Encoding.UTF8.GetBytes(friend.Remarks),
                PersonalSign = Encoding.UTF8.GetBytes(friend.PersonalSign),
                Qid = Encoding.UTF8.GetBytes(friend.Qid),
                Category = friend.Category
            };
        }

        
    }
}
