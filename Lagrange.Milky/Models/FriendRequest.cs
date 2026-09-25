using System.Text.Json.Serialization;

namespace Lagrange.Milky.Models;

public class FriendRequest
{
    [JsonPropertyName("time")] public required long Time { get; init; }
    [JsonPropertyName("initiator_id")] public required long InitiatorId { get; init; }
    [JsonPropertyName("initiator_uid")] public required string InitiatorUid { get; init; }
    [JsonPropertyName("target_user_id")] public required long TargetUserId { get; init; }
    [JsonPropertyName("target_user_uid")] public required string TargetUserUid { get; init; }
    [JsonPropertyName("state")] public required string State { get; init; }
    [JsonPropertyName("comment")] public required string Comment { get; init; }
    [JsonPropertyName("via")] public required string Via { get; init; }
    [JsonPropertyName("is_filtered")] public required bool IsFiltered { get; init; }
}
