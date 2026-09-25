using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotGroupStruct
    {
        public BotGroupStruct() { }

        public long GroupUin = 0;

        public ByteArrayNative GroupName = new();

        public int MemberCount = 0;
        
        public int MaxMember = 0;

        public long CreateTime = 0;

        public ByteArrayNative Description = new();

        public ByteArrayNative Question = new();

        public ByteArrayNative Announcement = new();
        
        public static implicit operator BotGroup(BotGroupStruct group)
        {
            return new BotGroup(
                group.GroupUin,
                Encoding.UTF8.GetString(group.GroupName),
                group.MemberCount,
                group.MaxMember,
                group.CreateTime,
                Encoding.UTF8.GetString(group.Description),
                Encoding.UTF8.GetString(group.Question),
                Encoding.UTF8.GetString(group.Announcement)
            );
        }
        
        public static implicit operator BotGroupStruct(BotGroup group)
        {
            return new BotGroupStruct()
            {
                GroupUin = group.GroupUin,
                GroupName = Encoding.UTF8.GetBytes(group.GroupName),
                MemberCount = group.MemberCount,
                MaxMember = group.MaxMember,
                CreateTime = group.CreateTime,
                Description = Encoding.UTF8.GetBytes(group.Description ?? string.Empty),
                Question = Encoding.UTF8.GetBytes(group.Question ?? string.Empty),
                Announcement = Encoding.UTF8.GetBytes(group.Announcement ?? string.Empty)
            };
        }
        
        
    }
}