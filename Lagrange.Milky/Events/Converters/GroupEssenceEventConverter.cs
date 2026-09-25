using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class GroupEssenceEventConverter : IEventConverter<BotGroupEssenceEvent, GroupEssenceEventConverter.Data>
{
    public string Name => "group_essence_message_change";

    public bool CanConvert(BotGroupEssenceEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotGroupEssenceEvent @event, CancellationToken ct) =>
        ValueTask.FromResult(new Data
        {
            GroupId = @event.GroupUin,
            MessageSeq = (long)@event.MessageSequence,
            OperatorId = @event.OperatorUin,
            IsSet = @event.IsSet,
        });

    public class Data
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("message_seq")] public required long MessageSeq { get; init; }
        [JsonPropertyName("operator_id")] public required long OperatorId { get; init; }
        [JsonPropertyName("is_set")] public required bool IsSet { get; init; }
    }
}
