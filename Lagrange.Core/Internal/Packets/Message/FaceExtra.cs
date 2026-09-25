using Lagrange.Proto;

#pragma warning disable CS8618 
namespace Lagrange.Core.Internal.Packets.Message;




[ProtoPackable]
internal partial class QBigFaceExtra
{
    [ProtoMember(1)] public string? AniStickerPackId { get; set; }

    [ProtoMember(2)] public string? AniStickerId { get; set; }

    [ProtoMember(3)] public int FaceId { get; set; }

    [ProtoMember(4)] public int Field4 { get; set; }

    [ProtoMember(5)] public int AniStickerType { get; set; }

    [ProtoMember(6)] public string? Field6 { get; set; }

    [ProtoMember(7)] public string? Preview { get; set; }

    [ProtoMember(9)] public int Field9 { get; set; }
}




[ProtoPackable]
internal partial class QSmallFaceExtra
{
    [ProtoMember(1)] public uint FaceId { get; set; }

    [ProtoMember(2)] public string? Text { get; set; }

    [ProtoMember(3)] public string? CompatText { get; set; }
}
