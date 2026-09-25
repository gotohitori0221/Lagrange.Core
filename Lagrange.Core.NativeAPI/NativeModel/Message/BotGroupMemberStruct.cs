using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotGroupMemberStruct
    {
        public BotGroupMemberStruct() { }

        public BotGroupStruct BotGroup = new();
        
        public long Uin = 0;

        public ByteArrayNative Uid = new();

        public ByteArrayNative Nickname = new();

        public int Age = 0;

        public int Gender = 0;

        public int Permission = 0;

        public int GroupLevel = 0;

        public ByteArrayNative MemberCard = new();

        public ByteArrayNative SpecialTitle = new();

        public long JoinTime = 0;

        public long LastMsgTime = 0;

        public long ShutUpTimestamp = 0;


        public static implicit operator BotGroupMember(BotGroupMemberStruct member)
        {
            return new BotGroupMember(
                member.BotGroup,
                member.Uin,
                Encoding.UTF8.GetString(member.Uid),
                Encoding.UTF8.GetString(member.Nickname),
                (GroupMemberPermission)member.Permission,
                member.GroupLevel,
                Encoding.UTF8.GetString(member.MemberCard),
                Encoding.UTF8.GetString(member.SpecialTitle),
                member.JoinTime,
                member.LastMsgTime,
                member.ShutUpTimestamp
            );
        }

        public static implicit operator BotGroupMemberStruct(BotGroupMember member)
        {
            return new BotGroupMemberStruct()
            {
                BotGroup = member.Group,
                Uin = member.Uin,
                Uid = Encoding.UTF8.GetBytes(member.Uid),
                Nickname = Encoding.UTF8.GetBytes(member.Nickname),
                Age = member.Age,
                Gender = (int)member.Gender,
                Permission = (int)member.Permission,
                GroupLevel = member.GroupLevel,
                MemberCard = Encoding.UTF8.GetBytes(member.MemberCard ?? string.Empty),
                SpecialTitle = Encoding.UTF8.GetBytes(member.SpecialTitle ?? string.Empty),
                JoinTime = member.JoinTime,
                LastMsgTime = member.LastMsgTime,
                ShutUpTimestamp = member.ShutUpTimestamp
            };
        }
    }
}
