using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618




[ProtoPackable]
internal partial class D126BReqBody
{
    [ProtoMember(1)] public D126BReqBodyField1 Field1 { get; set; } = new();
}

[ProtoPackable]
internal partial class D126BReqBodyField1
{
    [ProtoMember(1)] public string? TargetUid { get; set; }

    [ProtoMember(2)] public D126BReqBodyField1_2 Field2 { get; set; } = new();

    [ProtoMember(3)] public bool Block { get; set; }

    [ProtoMember(4)] public bool Field4 { get; set; }
}

[ProtoPackable]
internal partial class D126BReqBodyField1_2
{
    [ProtoMember(1)] public uint Field1 { get; set; } = 130;

    [ProtoMember(2)] public uint Field2 { get; set; } = 109;

    [ProtoMember(3)] public D126BReqBodyField1_2_3 Field3 { get; set; } = new();
}

[ProtoPackable]
internal partial class D126BReqBodyField1_2_3
{
    [ProtoMember(1)] public uint Field1 { get; set; } = 8;

    [ProtoMember(2)] public uint Field2 { get; set; } = 8;

    [ProtoMember(3)] public uint Field3 { get; set; } = 50;
}
