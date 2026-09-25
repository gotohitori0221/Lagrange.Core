using Lagrange.Codec.Interop;

namespace Lagrange.Codec.Streams;

public class SilkEncodeStream() : NativeCodecStream(AudioInterop.SilkEncode);

public class SilkDecodeStream() : NativeCodecStream(AudioInterop.SilkDecode);