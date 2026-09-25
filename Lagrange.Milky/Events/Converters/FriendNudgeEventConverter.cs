using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class FriendNudgeEventConverter : IEventConverter<BotFriendNudgeEvent, FriendNudgeEventConverter.Data>
{
    public string Name => "friend_nudge";

    public bool CanConvert(BotFriendNudgeEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotFriendNudgeEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        UserId = @event.PeerUin,
        IsSelfSend = @event.IsSelfSend,
        IsSelfReceive = @event.IsSelfReceive,
        DisplayAction = @event.Action,
        DisplaySuffix = @event.Suffix,
        DisplayActionImgUrl = @event.ActionImageUrl,
    });

    public class Data
    {
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
        [JsonPropertyName("is_self_send")] public required bool IsSelfSend { get; init; }
        [JsonPropertyName("is_self_receive")] public required bool IsSelfReceive { get; init; }
        [JsonPropertyName("display_action")] public required string DisplayAction { get; init; }
        [JsonPropertyName("display_suffix")] public required string DisplaySuffix { get; init; }
        [JsonPropertyName("display_action_img_url")] public required string DisplayActionImgUrl { get; init; }
    }
}
