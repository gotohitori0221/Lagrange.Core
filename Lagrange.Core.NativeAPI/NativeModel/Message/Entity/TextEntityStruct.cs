using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Message.Entities;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message.Entity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TextEntityStruct : IEntityStruct
    {
        public TextEntityStruct() { }
        
        public ByteArrayNative Text = new();
        
        public static implicit operator TextEntityStruct(TextEntity entity)
        {
            return new TextEntityStruct() { Text = Encoding.UTF8.GetBytes(entity.Text) };
        }
    }
}