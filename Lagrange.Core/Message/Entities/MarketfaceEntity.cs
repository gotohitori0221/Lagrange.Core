using System.Collections.Generic;
using System.Text;
using Lagrange.Core.Internal.Packets.Message;

namespace Lagrange.Core.Message.Entities;

public class MarketfaceEntity : IMessageEntity
{
    public string EmojiId { get; set; }

    public int EmojiPackageId { get; set; }

    public string Key { get; set; }

    public string Summary { get; set; }

    public MarketfaceEntity() : this(string.Empty, default, string.Empty, string.Empty) { }

    public MarketfaceEntity(string emojiId, int emojiPackageId, string key, string summary)
    {
        EmojiId = emojiId;
        EmojiPackageId = emojiPackageId;
        Key = key;
        Summary = summary;
    }

    Elem[] IMessageEntity.Build()
    {
        return
        [
            new Elem
            {
                Marketface = new Marketface
                {
                    Summary = Summary,
                    ItemType = 6,
                    Info = 1,
                    FaceId = Encoding.ASCII.GetBytes(EmojiId),
                    TabId = EmojiPackageId,
                    SubType = 3,
                    Key = Key,
                    Width = 300,
                    Height = 300,
                    PbReserve = new MarketfaceReserve
                    {
                        Field8 = 1
                    }
                }
            }
        ];
    }

    IMessageEntity? IMessageEntity.Parse(List<Elem> elements, Elem target)
    {
        if (target.Marketface is not { } marketface) return null;

        return new MarketfaceEntity(
            marketface.FaceId is { Length: > 0 } ? Encoding.ASCII.GetString(marketface.FaceId) : string.Empty,
            marketface.TabId,
            marketface.Key ?? string.Empty,
            marketface.Summary ?? string.Empty
        );
    }

    string IMessageEntity.ToPreviewString() => $"[MarketFace]: {EmojiPackageId}; {EmojiId}; {Key}; {Summary}";
}
