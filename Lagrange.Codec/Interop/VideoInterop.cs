using System.Runtime.InteropServices;
using Lagrange.Codec.Entities;

namespace Lagrange.Codec.Interop;

internal static partial class VideoInterop
{
    [LibraryImport("LagrangeCodec", EntryPoint = "video_first_frame")]
    public static partial int VideoFirstFrame(
        IntPtr videoData,
        int dataLen,
        ref IntPtr output,
        ref int outputLen);
    
    [LibraryImport("LagrangeCodec", EntryPoint = "video_get_size")]
    public static partial int VideoGetSize(
        IntPtr videoData,
        int dataLen,
        ref VideoInfo output);
}