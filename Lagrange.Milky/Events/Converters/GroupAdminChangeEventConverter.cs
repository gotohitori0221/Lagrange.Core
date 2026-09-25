using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class GroupAdminChangeEventConverter : IEventConverter<BotGroupAdminChangeEvent, GroupAdminChangeEventConverter.Data>
{
    public string Name => "group_admin_change";

    public bool CanConvert(BotGroupAdminChangeEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotGroupAdminChangeEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        GroupId = @event.GroupUin,
        UserId = @event.TargetUin,
        OperatorId = @event.OperatorUin,
        IsSet = @event.IsAdmin,
    });

    public class Data
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
        [JsonPropertyName("operator_id")] public required long OperatorId { get; init; }
        [JsonPropertyName("is_set")] public required bool IsSet { get; init; }
    }
}
