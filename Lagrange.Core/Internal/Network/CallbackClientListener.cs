namespace Lagrange.Core.Internal.Network;

internal sealed class CallbackClientListener(IClientListener listener) : ClientListener
{
    public override uint HeaderSize => listener.HeaderSize;

    public override uint GetPacketLength(ReadOnlySpan<byte> header) => listener.GetPacketLength(header);

    public override void OnDisconnect() => listener.OnDisconnect();

    public override void OnRecvPacket(ReadOnlySpan<byte> packet) => listener.OnRecvPacket(packet);

    public override void OnSocketError(Exception e, ReadOnlyMemory<byte> data = default) => listener.OnSocketError(e, data);
}