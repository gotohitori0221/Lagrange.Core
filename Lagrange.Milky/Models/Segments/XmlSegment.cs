using System.Text.Json.Serialization;

namespace Lagrange.Milky.Models.Segments;

public class XmlIncomingSegment : IncomingSegmentBase<XmlIncomingSegmentData>;
public class XmlIncomingSegmentData
{
    [JsonPropertyName("service_id")] public required int ServiceId { get; init; }
    [JsonPropertyName("xml_payload")] public required string XmlPayload { get; init; }
}
