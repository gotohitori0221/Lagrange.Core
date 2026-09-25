using System.Runtime.InteropServices;

namespace Lagrange.Core.NativeAPI.NativeModel.Message
{
    [StructLayout(LayoutKind.Sequential)]
    public class TypedEntityStruct
    {
        
        public IntPtr Entity = IntPtr.Zero;
        public int Type = 0;
    }
}