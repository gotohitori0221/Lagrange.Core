using Lagrange.Core.Events;

namespace Lagrange.Core.Internal.Events.System;

internal class Oidb0Xb77EventReq(ReadOnlyMemory<byte> body) : ProtocolEvent
{
    public ReadOnlyMemory<byte> Body { get; } = body;
}

internal class Oidb0Xb77EventResp(uint result, string message, ReadOnlyMemory<byte> body) : ProtocolEvent
{
    public uint Result { get; } = result;

    public string Message { get; } = message;

    public ReadOnlyMemory<byte> Body { get; } = body;
}
