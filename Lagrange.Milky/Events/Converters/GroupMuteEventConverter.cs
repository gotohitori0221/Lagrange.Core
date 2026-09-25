using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class GroupMuteEventConverter : IEventConverter<BotGroupMuteEvent, GroupMuteEventConverter.Data>
{
    public string Name => "group_mute";

    public bool CanConvert(BotGroupMuteEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotGroupMuteEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        GroupId = @event.GroupUin,
        UserId = @event.TargetUin,
        OperatorId = @event.OperatorUin,
        Duration = @event.Duration > int.MaxValue ? int.MaxValue : (int)@event.Duration,
    });

    public class Data
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
        [JsonPropertyName("operator_id")] public required long OperatorId { get; init; }
        [JsonPropertyName("duration")] public required int Duration { get; init; }
    }
}
