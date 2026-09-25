using Lagrange.Core.Internal.Packets.Message;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Message.Entities;

public class FaceEntity : IMessageEntity
{
    public ushort FaceId { get; set; }

    public bool IsLargeFace { get; set; }

    public FaceEntity() { }

    public FaceEntity(ushort faceId, bool isLargeFace)
    {
        FaceId = faceId;
        IsLargeFace = isLargeFace;
    }

    Elem[] IMessageEntity.Build()
    {
        if (IsLargeFace)
        {
            var qBigFace = new QBigFaceExtra
            {
                AniStickerPackId = "1",
                AniStickerId = "8",
                FaceId = FaceId,
                Field4 = 1,
                AniStickerType = 1,
                Field6 = string.Empty,
                Preview = string.Empty,
                Field9 = 1
            };
            return
            [
                new Elem
                {
                    CommonElem = new CommonElem
                    {
                        ServiceType = 37,
                        PbElem = ProtoHelper.Serialize(qBigFace),
                        BusinessType = 1
                    }
                }
            ];
        }

        if (FaceId >= 260)
        {
            var qSmallFace = new QSmallFaceExtra
            {
                FaceId = FaceId,
                Text = string.Empty,
                CompatText = string.Empty
            };
            return
            [
                new Elem
                {
                    CommonElem = new CommonElem
                    {
                        ServiceType = 33,
                        PbElem = ProtoHelper.Serialize(qSmallFace),
                        BusinessType = 1
                    }
                }
            ];
        }

        return [new Elem { Face = new Face { Index = FaceId } }];
    }

    IMessageEntity? IMessageEntity.Parse(List<Elem> elements, Elem target)
    {
        if (target.Face is { Index: not null } face)
            return new FaceEntity((ushort)face.Index, false);

        if (target.CommonElem is { ServiceType: 37, PbElem.Length: > 0 } big)
        {
            var qBigFace = ProtoHelper.Deserialize<QBigFaceExtra>(big.PbElem.Span);
            return new FaceEntity((ushort)qBigFace.FaceId, true);
        }

        if (target.CommonElem is { ServiceType: 33, PbElem.Length: > 0 } small)
        {
            var qSmallFace = ProtoHelper.Deserialize<QSmallFaceExtra>(small.PbElem.Span);
            return new FaceEntity((ushort)qSmallFace.FaceId, false);
        }

        return null;
    }

    string IMessageEntity.ToPreviewString() => $"[Face][{(IsLargeFace ? "Large" : "Small")}]: {FaceId}";
}
