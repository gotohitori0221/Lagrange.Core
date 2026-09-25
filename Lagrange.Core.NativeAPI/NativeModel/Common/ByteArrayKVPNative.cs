using System.Runtime.InteropServices;


namespace Lagrange.Core.NativeAPI.NativeModel.Common;

[StructLayout(LayoutKind.Sequential)]
public struct ByteArrayKVPNative
{
    public ByteArrayNative Key;
    public ByteArrayNative Value;
}