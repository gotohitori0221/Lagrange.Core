using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Message.Entities;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message.Entity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FileEntityStruct : IEntityStruct
    {
        public FileEntityStruct() { }

        public ByteArrayNative FileId = new();

        public ByteArrayNative FileName = new();

        public long FileSize = 0;

        public ByteArrayNative FileMd5 = new();

        public ByteArrayNative FileHash = new();

        public static implicit operator FileEntityStruct(FileEntity entity)
        {
            return new FileEntityStruct()
            {
                FileId = Encoding.UTF8.GetBytes(entity.FileId),
                FileName = Encoding.UTF8.GetBytes(entity.FileName),
                FileSize = entity.FileSize,
                FileMd5 = Encoding.UTF8.GetBytes(entity.FileMd5),
                FileHash = Encoding.UTF8.GetBytes(entity.FileHash ?? string.Empty),
            };
        }
    }
}