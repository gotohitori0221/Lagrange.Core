using Lagrange.Codec.Interop;

namespace Lagrange.Codec.Streams;

public class PCMStream() : NativeCodecStream(AudioInterop.AudioToPCM);