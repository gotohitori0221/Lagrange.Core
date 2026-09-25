using System.Runtime.InteropServices;

namespace Lagrange.Core.NativeAPI.NativeModel.Common;

public struct ByteArrayDictNative
{
    public int Length;
    public IntPtr Data;
    
    public static implicit operator ByteArrayDictNative(ByteArrayKVPNative[] dict)
    {
        int size = Marshal.SizeOf<ByteArrayKVPNative>();
        IntPtr ptr = Marshal.AllocHGlobal(size * dict.Length);
        for (int i = 0; i < dict.Length; i++)
        {
            Marshal.StructureToPtr(dict[i], ptr + i * size, false);
        }
        
        return new ByteArrayDictNative { Length = dict.Length, Data = ptr };
    }
    
    public static implicit operator ByteArrayKVPNative[](ByteArrayDictNative dict)
    {
        if (dict.Data == IntPtr.Zero || dict.Length == 0)
        {
            return [];
        }
        
        ByteArrayKVPNative[] result = new ByteArrayKVPNative[dict.Length];
        int size = Marshal.SizeOf<ByteArrayKVPNative>();
        for (int i = 0; i < dict.Length; i++)
        {
            result[i] = Marshal.PtrToStructure<ByteArrayKVPNative>(dict.Data + i * size);
        }
        
        Marshal.FreeHGlobal(dict.Data);
        
        return result;
    }
}