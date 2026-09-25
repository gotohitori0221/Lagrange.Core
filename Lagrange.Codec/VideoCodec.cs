using System.Numerics;
using System.Runtime.InteropServices;
using Lagrange.Codec.Entities;
using Lagrange.Codec.Interop;

namespace Lagrange.Codec;

public static class VideoCodec
{
    public static byte[] FirstFrame(byte[] video)
    {
        var handle = Marshal.AllocHGlobal(video.Length);
        Marshal.Copy(video, 0, handle, video.Length);
        
        int length = 0;
        var outPtr = IntPtr.Zero;
        int result = VideoInterop.VideoFirstFrame(handle, video.Length, ref outPtr, ref length);
        Marshal.FreeHGlobal(handle);
        
        if (result != 0)
        {
            if (outPtr != IntPtr.Zero) Marshal.FreeHGlobal(outPtr);
            throw new Exception("Failed to get first frame");
        }
        
        var output = new byte[length];
        Marshal.Copy(outPtr, output, 0, length);
        Marshal.FreeHGlobal(outPtr);
        
        GC.Collect();
        GC.WaitForPendingFinalizers();
        
        return output;
    }
    
    public static VideoInfo GetSize(byte[] video)
    {
        var handle = Marshal.AllocHGlobal(video.Length);
        Marshal.Copy(video, 0, handle, video.Length);
        
        var result = new VideoInfo();
        int code = VideoInterop.VideoGetSize(handle, video.Length, ref result);
        if (code != 0) throw new Exception("Failed to get video size");
        
        Marshal.FreeHGlobal(handle);
        
        GC.Collect();
        GC.WaitForPendingFinalizers();
        
        return result;
    }
}