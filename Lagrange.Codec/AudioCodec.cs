using Lagrange.Codec.Exceptions;
using Lagrange.Codec.Streams;

namespace Lagrange.Codec;

public static class AudioCodec
{
    public static async Task<(byte[], float)> EncodeSilkV3(byte[] raw)
    {
        var format = AudioHelper.DetectAudio(raw);

        switch (format)
        {
            case AudioFormat.Amr:
            {
                return (raw, raw.Length / 1607f);
            }
            case AudioFormat.TenSilkV3:
            {
                return (raw, AudioHelper.GetTenSilkTime(raw));
            }
            case AudioFormat.SilkV3:
            {
                return ([0x02, ..raw[..^2]], AudioHelper.GetSilkTime(raw)); 
            }
            default:
            {
                var input = new MemoryStream(raw);
                var output = new MemoryStream();

                MemoryStream[] pipe = [input, new PCMStream(), new SilkEncodeStream(), output];
                
                for (int i = 0; i < pipe.Length - 1; i++)
                {
                    await pipe[i].CopyToAsync(pipe[i + 1]);
                    await pipe[i].DisposeAsync();
                }
                
                var result = output.ToArray();
                return (result, AudioHelper.GetTenSilkTime(result));
            }
        }
    }
}