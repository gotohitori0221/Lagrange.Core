using System.Runtime.InteropServices;

namespace Lagrange.Core.NativeAPI.Test;

public static class Wrapper
{
    public const string DLL_NAME = "Lagrange.Core.NativeAPI.dll";
    
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Initialize(IntPtr botConfigPtr, IntPtr keystorePtr, IntPtr appConfigPtr);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Start(int index);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int Stop(int index);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int FreeMemory(IntPtr ptr);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetQrCodeEvent(int index);
    
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetEventCount(int index);
    
    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetBotLogEvent(int index);
}