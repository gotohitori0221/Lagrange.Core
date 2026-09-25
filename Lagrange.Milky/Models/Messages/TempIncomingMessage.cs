using System.Text.Json.Serialization;

namespace Lagrange.Milky.Models.Messages;

public class TempIncomingMessage : IncomingMessageBase
{
    [JsonPropertyName("group")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Group? Group { get; init; }
}
