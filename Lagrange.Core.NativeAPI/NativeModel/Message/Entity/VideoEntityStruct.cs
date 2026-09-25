using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Message.Entities;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message.Entity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VideoEntityStruct : IEntityStruct
    {
        public VideoEntityStruct() { }
        
        public ByteArrayNative FileUrl = new();

        public ByteArrayNative FileName = new();

        public ByteArrayNative FileSha1 = new();

        public uint FileSize = 0;

        public ByteArrayNative FileMd5 = new();
        
        public static implicit operator VideoEntityStruct(VideoEntity entity)
        {
            return new VideoEntityStruct()
            {
                FileUrl = Encoding.UTF8.GetBytes(entity.FileUrl),
                FileName = Encoding.UTF8.GetBytes(entity.FileName),
                FileSha1 = Encoding.UTF8.GetBytes(entity.FileSha1),
                FileSize = entity.FileSize,
                FileMd5 = Encoding.UTF8.GetBytes(entity.FileMd5)
            };
        }
    }
}