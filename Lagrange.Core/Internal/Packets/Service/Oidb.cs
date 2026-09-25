using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

[ProtoPackable]
internal partial class Oidb
{
    [ProtoMember(1)] public uint Command { get; set; }
    
    [ProtoMember(2)] public uint Service { get; set; }
    
    [ProtoMember(3)] public uint Result { get; set; }

    [ProtoMember(4)] public ReadOnlyMemory<byte> Body { get; set; }
    
    [ProtoMember(5)] public string Message { get; set; } = string.Empty;
    
    [ProtoMember(12)] public uint Reserved { get; set; }
}