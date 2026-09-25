using System.Text.Json.Serialization;

namespace Lagrange.Milky.Models.Messages;

public class GroupIncomingMessage : IncomingMessageBase
{
    

    [JsonPropertyName("group")] public required Group Group { get; init; }
    [JsonPropertyName("group_member")] public required GroupMember GroupMember { get; init; }
}
