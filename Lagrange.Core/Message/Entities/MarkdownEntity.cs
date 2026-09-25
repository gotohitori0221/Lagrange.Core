using System.Collections.Generic;
using Lagrange.Core.Internal.Packets.Message;
using Lagrange.Core.Utility;
using Lagrange.Proto;

namespace Lagrange.Core.Message.Entities;

public class MarkdownEntity : IMessageEntity
{
    public MarkdownData Data { get; set; } = new();

    public MarkdownEntity() { }

    public MarkdownEntity(MarkdownData data) => Data = data;

    public MarkdownEntity(string content) => Data = new MarkdownData { Content = content };

    Elem[] IMessageEntity.Build()
    {
        return
        [
            new Elem
            {
                CommonElem = new CommonElem
                {
                    ServiceType = 45,
                    PbElem = ProtoHelper.Serialize(Data),
                    BusinessType = 1,
                }
            }
        ];
    }

    IMessageEntity? IMessageEntity.Parse(List<Elem> elements, Elem target)
    {
        if (target.CommonElem is { ServiceType: 45, BusinessType: 1, PbElem.Length: > 0 } common)
        {
            return new MarkdownEntity(ProtoHelper.Deserialize<MarkdownData>(common.PbElem.Span));
        }

        return null;
    }

    string IMessageEntity.ToPreviewString() => $"[Markdown]: {Data.Content}";
}

[ProtoPackable]
public partial class MarkdownData
{
    [ProtoMember(1)] public string Content { get; set; } = string.Empty;
}
