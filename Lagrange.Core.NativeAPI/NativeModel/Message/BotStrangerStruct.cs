using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BotStrangerStruct
    {
        public BotStrangerStruct() { }
        public long Uin = 0;
        public ByteArrayNative Nickname = new();
        public ByteArrayNative Uid = new();
        public ByteArrayNative PersonalSign = new();
        public ByteArrayNative Remark = new();
        public ulong Level = 0;
        public int Gender = 0;
        public long RegistrationTime = 0;
        public long Birthday = 0;
        public ulong Age = 0;
        public ByteArrayNative QID = new();
        public long Source = 0;
        public ByteArrayNative Country = new();
        public ByteArrayNative City = new();
        public ByteArrayNative School = new();

        public static implicit operator BotStranger(BotStrangerStruct stranger)
        {
            return new BotStranger(
                stranger.Uin,
                Encoding.UTF8.GetString(stranger.Nickname),
                Encoding.UTF8.GetString(stranger.Uid),
                Encoding.UTF8.GetString(stranger.PersonalSign),
                Encoding.UTF8.GetString(stranger.Remark),
                stranger.Level,
                (BotGender)stranger.Gender,
                stranger.RegistrationTime,
                stranger.Birthday,
                stranger.Age,
                Encoding.UTF8.GetString(stranger.QID),
                Encoding.UTF8.GetString(stranger.Country),
                Encoding.UTF8.GetString(stranger.City),
                Encoding.UTF8.GetString(stranger.School)
            );
        }

        public static implicit operator BotStrangerStruct(BotStranger stranger)
        {
            return new BotStrangerStruct()
            {
                Uin = stranger.Uin,
                Nickname = Encoding.UTF8.GetBytes(stranger.Nickname),
                Uid = Encoding.UTF8.GetBytes(stranger.Uid),
                PersonalSign = Encoding.UTF8.GetBytes(stranger.PersonalSign),
                Remark = Encoding.UTF8.GetBytes(stranger.Remark),
                Level = stranger.Level,
                Gender = (int)stranger.Gender,
                RegistrationTime = stranger.RegistrationTime,
                Birthday = stranger.Birthday ?? 0,
                Age = stranger.Age,
                QID = Encoding.UTF8.GetBytes(stranger.QID),
                Country = Encoding.UTF8.GetBytes(stranger.Country),
                City = Encoding.UTF8.GetBytes(stranger.City),
                School = Encoding.UTF8.GetBytes(stranger.School ?? string.Empty)
            };
        }
    }
}