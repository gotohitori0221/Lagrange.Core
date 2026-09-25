using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class GroupNameChangeEventConverter : IEventConverter<BotGroupNameChangeEvent, GroupNameChangeEventConverter.Data>
{
    public string Name => "group_name_change";

    public bool CanConvert(BotGroupNameChangeEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotGroupNameChangeEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        GroupId = @event.GroupUin,
        NewGroupName = @event.NewName,
        OperatorId = @event.OperatorUin,
    });

    public class Data
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("new_group_name")] public required string NewGroupName { get; init; }
        [JsonPropertyName("operator_id")] public required long OperatorId { get; init; }
    }
}
