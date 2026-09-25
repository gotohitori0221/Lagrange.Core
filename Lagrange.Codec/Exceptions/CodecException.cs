namespace Lagrange.Codec.Exceptions;

public class CodecException(string message, Exception? innerException = null) : Exception(message, innerException)
{
    public CodecException(Exception innerException) : this(innerException.Message, innerException) { }
}