using System.Runtime.InteropServices;
using System.Text;
using Lagrange.Core.Message.Entities;
using Lagrange.Core.NativeAPI.NativeModel.Common;

namespace Lagrange.Core.NativeAPI.NativeModel.Message.Entity
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ImageEntityStruct : IEntityStruct
    {
        public ImageEntityStruct() { }
        
        public ByteArrayNative FileUrl = new();
        
        public ByteArrayNative FileName = new();
        
        public ByteArrayNative FileSha1 = new();
        
        public uint FileSize = 0;
        
        public ByteArrayNative FileMd5 = new();
        
        public float ImageWidth = 0;
        
        public float ImageHeight = 0;
        
        public int SubType = 0;
        
        public ByteArrayNative Summary = new();

        public uint RecordLength = 0;
        
        public static implicit operator ImageEntityStruct(ImageEntity entity)
        {
            return new ImageEntityStruct()
            {
                FileUrl = Encoding.UTF8.GetBytes(entity.FileUrl),
                FileName = Encoding.UTF8.GetBytes(entity.FileName),
                FileSha1 = Encoding.UTF8.GetBytes(entity.FileSha1),
                FileSize = entity.FileSize,
                FileMd5 = Encoding.UTF8.GetBytes(entity.FileMd5),
                ImageWidth = entity.ImageSize.X,
                ImageHeight = entity.ImageSize.Y,
                SubType = entity.SubType,
                Summary = Encoding.UTF8.GetBytes(entity.Summary)
            };
        }
    }
}