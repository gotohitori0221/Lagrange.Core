using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618




[ProtoPackable]
internal partial class DB5DReqBody
{
    
    [ProtoMember(1)] public uint Accept { get; set; }

    [ProtoMember(2)] public string TargetUid { get; set; }
}
