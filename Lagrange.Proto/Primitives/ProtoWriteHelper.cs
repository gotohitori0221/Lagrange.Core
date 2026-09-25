using System.Buffers;
using System.Diagnostics;
using System.Text.Unicode;

namespace Lagrange.Proto.Primitives;

internal static class ProtoWriteHelper
{
    internal static OperationStatus ToUtf8(ReadOnlySpan<char> source, Span<byte> destination, out int written)
    {
        var status = Utf8.FromUtf16(source, destination, out int charsRead, out written, replaceInvalidSequences: false, isFinalBlock: true);
        Debug.Assert(status is OperationStatus.Done or OperationStatus.DestinationTooSmall or OperationStatus.InvalidData);
        Debug.Assert(charsRead == source.Length || status is not OperationStatus.Done);
        return status;
    }
}