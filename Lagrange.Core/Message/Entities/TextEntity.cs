using Lagrange.Core.Internal.Packets.Message;

namespace Lagrange.Core.Message.Entities;

public class TextEntity(string text) : IMessageEntity
{
    public string Text { get; } = text;

    public TextEntity() : this(string.Empty) { }
    
    Elem[] IMessageEntity.Build()
    {
        return
        [
            new Elem { Text = new Text { TextMsg = Text } }
        ];
    }
    
    string IMessageEntity.ToPreviewString() => Text;
    
    IMessageEntity? IMessageEntity.Parse(List<Elem> elements, Elem target)
    {
        if (target.Text is not ({ Attr6Buf: null } or { Attr6Buf.Length: 0 }) or not ({ PbReserve.Length: 0 }))
            return null;

        
        
        int idx = elements.IndexOf(target);
        if (idx >= 0 && idx + 1 < elements.Count)
        {
            if (elements[idx + 1].CommonElem is { ServiceType: 45, BusinessType: 1 }) return null;
        }

        return new TextEntity(target.Text.TextMsg);
    }
}