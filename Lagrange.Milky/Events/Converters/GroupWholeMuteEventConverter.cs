using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class GroupWholeMuteEventConverter : IEventConverter<BotGroupWholeMuteEvent, GroupWholeMuteEventConverter.Data>
{
    public string Name => "group_whole_mute";

    public bool CanConvert(BotGroupWholeMuteEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotGroupWholeMuteEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        GroupId = @event.GroupUin,
        OperatorId = @event.OperatorUin,
        IsMute = @event.IsMuted,
    });

    public class Data
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("operator_id")] public required long OperatorId { get; init; }
        [JsonPropertyName("is_mute")] public required bool IsMute { get; init; }
    }
}
