using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Message;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class MsgPush
{
    [ProtoMember(1)] public CommonMessage CommonMessage { get; set; }
    
    [ProtoMember(5)] public bool PushNotifyFlag { get; set; }
}