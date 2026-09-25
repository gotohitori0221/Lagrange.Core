using System.Text.Json.Serialization;

namespace Lagrange.Milky.Models.Segments;

public class FaceIncomingSegment : IncomingSegmentBase<FaceIncomingSegmentData>;
public class FaceIncomingSegmentData
{
    [JsonPropertyName("face_id")] public required string FaceId { get; init; }
    [JsonPropertyName("is_large")] public required bool IsLarge { get; init; }
}

public sealed class FaceOutgoingSegment : OutgoingSegmentBase<FaceOutgoingSegmentData>;
public sealed class FaceOutgoingSegmentData(string faceId, bool isLarge = false)
{
    [JsonPropertyName("face_id")] public required string FaceId { get; init; } = faceId;
    [JsonPropertyName("is_large")] public bool IsLarge { get; init; } = isLarge;
}
