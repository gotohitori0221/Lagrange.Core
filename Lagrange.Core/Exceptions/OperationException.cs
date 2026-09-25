namespace Lagrange.Core.Exceptions;




public class OperationException(int result, string? errMsg = null) : LagrangeException($"Operation failed with code {result}: {errMsg}")
{
    public int Result { get; } = result;

    public string? ErrMsg { get; } = errMsg;
}