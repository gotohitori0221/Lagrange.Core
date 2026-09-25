using System.Text.Json.Serialization;

namespace Lagrange.Milky.Models.Messages;

public class FriendIncomingMessage : IncomingMessageBase
{
    

    [JsonPropertyName("friend")] public required Friend Friend { get; init; }
}
