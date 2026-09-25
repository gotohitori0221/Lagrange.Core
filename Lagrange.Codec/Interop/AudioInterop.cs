using System.Runtime.InteropServices;

namespace Lagrange.Codec.Interop;

internal static partial class AudioInterop
{
    public delegate void AudioCodecCallback(IntPtr userData, IntPtr p, int len);

    [LibraryImport("LagrangeCodec", EntryPoint = "audio_to_pcm")]
    public static partial int AudioToPCM(
        IntPtr audioData,
        int dataLen,
        AudioCodecCallback callback,
        IntPtr userdata);
    
    [LibraryImport("LagrangeCodec", EntryPoint = "silk_decode")]
    public static partial int SilkDecode(
        IntPtr silkData,
        int dataLen,
        AudioCodecCallback callback,
        IntPtr userdata);
    
    [LibraryImport("LagrangeCodec", EntryPoint = "silk_encode")]
    public static partial int SilkEncode(
        IntPtr pcmData,
        int dataLen,
        AudioCodecCallback callback,
        IntPtr userdata);
}