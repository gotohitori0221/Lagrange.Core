using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class GroupDisbandEventConverter : IEventConverter<BotGroupDisbandEvent, GroupDisbandEventConverter.Data>
{
    public string Name => "group_disband";

    public bool CanConvert(BotGroupDisbandEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotGroupDisbandEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        GroupId = @event.GroupUin,
        OperatorId = @event.OperatorUin ?? 0,
    });

    public class Data
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("operator_id")] public required long OperatorId { get; init; }
    }
}
