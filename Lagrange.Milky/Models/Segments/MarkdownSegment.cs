using System.Text.Json.Serialization;

namespace Lagrange.Milky.Models.Segments;

public class MarkdownIncomingSegment : IncomingSegmentBase<MarkdownIncomingSegmentData>;
public class MarkdownIncomingSegmentData
{
    [JsonPropertyName("content")] public required string Content { get; init; }
}
