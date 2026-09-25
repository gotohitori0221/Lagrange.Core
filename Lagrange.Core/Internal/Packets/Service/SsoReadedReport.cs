using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618




[ProtoPackable]
internal partial class SsoReadedReport
{
    [ProtoMember(1)] public SsoReadedReportGroup? Group { get; set; }

    [ProtoMember(2)] public SsoReadedReportC2C? C2C { get; set; }
}

[ProtoPackable]
internal partial class SsoReadedReportC2C
{
    [ProtoMember(2)] public string? TargetUid { get; set; }

    [ProtoMember(3)] public uint Time { get; set; }

    [ProtoMember(4)] public uint StartSequence { get; set; }
}

[ProtoPackable]
internal partial class SsoReadedReportGroup
{
    [ProtoMember(1)] public uint GroupUin { get; set; }

    [ProtoMember(2)] public uint StartSequence { get; set; }
}
