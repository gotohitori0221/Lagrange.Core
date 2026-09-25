namespace Lagrange.Core.Internal.Network;

internal interface IClientListener
{
    uint HeaderSize { get; }

    
    
    
    
    public uint GetPacketLength(ReadOnlySpan<byte> header);

    
    
    
    public void OnRecvPacket(ReadOnlySpan<byte> packet);

    
    
    
    public void OnDisconnect();

    
    
    
    public void OnSocketError(Exception e, ReadOnlyMemory<byte> data);
}