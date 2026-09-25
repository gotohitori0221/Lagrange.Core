using System.Runtime.InteropServices;
using Lagrange.Codec.Exceptions;
using Lagrange.Codec.Interop;

namespace Lagrange.Codec.Streams;

public abstract class NativeCodecStream : MemoryStream
{
    private bool _initialized;
    
    private readonly Func<nint, int, AudioInterop.AudioCodecCallback, nint, int> _encodeFunc;

    internal NativeCodecStream(Func<nint, int, AudioInterop.AudioCodecCallback, nint, int> encodeFunc)
    {
        _encodeFunc = encodeFunc;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!_initialized)
        {
            var source = ToArray();
            var native = Marshal.AllocHGlobal(source.Length);
            
            Marshal.Copy(source, 0, native, source.Length);
            
            SetLength(0);
            Position = 0;

            try
            {
                int result = _encodeFunc(native, source.Length, (_, p, len) =>
                {
                    var data = new byte[len];
                    Marshal.Copy(p, data, 0, len);
                    Write(data);
                }, IntPtr.Zero);

                if (result != 0) throw new CodecException($"Failed to encode silk data. Error code: {result}");

                Position = 0;
            }
            catch (CodecException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CodecException(e);
            }
            finally
            {
                Marshal.FreeHGlobal(native);
            }
            
            _initialized = true;
        }
        
        return base.Read(buffer, offset, count);
    }
}