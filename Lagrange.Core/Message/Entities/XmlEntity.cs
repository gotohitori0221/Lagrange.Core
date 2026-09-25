using System.Collections.Generic;
using System.Text;
using Lagrange.Core.Internal.Packets.Message;
using Lagrange.Core.Utility.Compression;

namespace Lagrange.Core.Message.Entities;

public class XmlEntity : IMessageEntity
{
    public string Xml { get; set; } = string.Empty;

    public int ServiceId { get; set; } = 35;

    private static ReadOnlySpan<byte> Header => new byte[1] { 0x01 };

    public XmlEntity() { }

    public XmlEntity(string xml) => Xml = xml;

    public XmlEntity(string xml, int serviceId) => (Xml, ServiceId) = (xml, serviceId);

    Elem[] IMessageEntity.Build()
    {
        return
        [
            new Elem
            {
                RichMsg = new RichMsg
                {
                    ServiceId = (uint)ServiceId,
                    BytesTemplate1 = ZCompression.ZCompress(Xml, Header.ToArray()),
                }
            }
        ];
    }

    IMessageEntity? IMessageEntity.Parse(List<Elem> elements, Elem target)
    {
        if (target.RichMsg is { ServiceId: 35, BytesTemplate1.Length: > 1 } richMsg)
        {
            var xml = ZCompression.ZDecompress(richMsg.BytesTemplate1.Span[1..]);
            return new XmlEntity(Encoding.UTF8.GetString(xml));
        }

        return null;
    }

    string IMessageEntity.ToPreviewString() => $"[Xml]: {Xml}";
}
