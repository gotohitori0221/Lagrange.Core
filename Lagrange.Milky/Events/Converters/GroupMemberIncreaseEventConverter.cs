using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class GroupMemberIncreaseEventConverter : IEventConverter<BotGroupMemberIncreaseEvent, GroupMemberIncreaseEventConverter.Data>
{
    public string Name => "group_member_increase";

    public bool CanConvert(BotGroupMemberIncreaseEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotGroupMemberIncreaseEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        GroupId = @event.GroupUin,
        UserId = @event.MemberUin,
        OperatorId = @event.OperatorUin == 0 ? null : @event.OperatorUin,
        InvitorId = @event.InvitorUin == 0 ? null : @event.InvitorUin,
    });

    public class Data
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
        [JsonPropertyName("operator_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? OperatorId { get; init; }
        [JsonPropertyName("invitor_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? InvitorId { get; init; }
    }
}