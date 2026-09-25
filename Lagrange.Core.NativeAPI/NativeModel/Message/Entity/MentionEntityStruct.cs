using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Message.Entities;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message.Entity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MentionEntityStruct : IEntityStruct
    {
        public MentionEntityStruct() { }

        public long Uin = 0;
        public ByteArrayNative Display = new();
        
        public static implicit operator MentionEntityStruct(MentionEntity entity)
        {
            return new MentionEntityStruct()
            {
                Uin = entity.Uin,
                Display = Encoding.UTF8.GetBytes(entity.Display ?? string.Empty)
            };
        }
    }
}
