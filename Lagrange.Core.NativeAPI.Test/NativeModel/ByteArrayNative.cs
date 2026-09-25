using System.Runtime.InteropServices;

namespace Lagrange.Core.NativeAPI.Test.NativeModel
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ByteArrayNative
    {
        public int Length;
        public IntPtr Data;

        public static implicit operator ByteArrayNative(byte[] bytes)
        {
            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);

            return new ByteArrayNative { Length = bytes.Length, Data = ptr };
        }
        
        public static implicit operator byte[](ByteArrayNative byteArray)
        {
            if (byteArray.Data == IntPtr.Zero)
            {
                return [];
            }
            byte[] bytes = new byte[byteArray.Length];
            Marshal.Copy(byteArray.Data, bytes, 0, byteArray.Length);
            
            Marshal.FreeHGlobal(byteArray.Data);
            
            return bytes;
        }

        public byte[] ToByteArrayWithoutFree()
        {
            byte[] bytes = new byte[Length];
            Marshal.Copy(Data, bytes, 0, Length);
            return bytes;
        }
        
        public bool IsEmpty()
        {
            return Length == 0 || Data == IntPtr.Zero;
        }
    }
}