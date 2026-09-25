using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class PeerPinChangeEventConverter : IEventConverter<BotPinChangedEvent, PeerPinChangeEventConverter.Data>
{
    public string Name => "peer_pin_change";

    public bool CanConvert(BotPinChangedEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotPinChangedEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        MessageScene = @event.ChatType switch
        {
            BotPinChangedEvent.PinChangedChatType.Friend => "friend",
            BotPinChangedEvent.PinChangedChatType.Group => "group",
            _ => @event.ChatType.ToString().ToLowerInvariant()
        },
        PeerId = @event.GroupUin ?? @event.Uin,
        IsPinned = @event.IsPin,
    });

    public class Data
    {
        [JsonPropertyName("message_scene")] public required string MessageScene { get; init; }
        [JsonPropertyName("peer_id")] public required long PeerId { get; init; }
        [JsonPropertyName("is_pinned")] public required bool IsPinned { get; init; }
    }
}