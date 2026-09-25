using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.System;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class SsoUnregister
{
    [ProtoMember(1)] public int RegType { get; set; }
    
    [ProtoMember(2)] public DeviceInfo DeviceInfo { get; set; }
    
    [ProtoMember(3)] public int UserTrigger { get; set; }
}