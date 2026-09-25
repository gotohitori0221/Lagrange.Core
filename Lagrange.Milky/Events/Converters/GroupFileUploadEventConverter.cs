using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class GroupFileUploadEventConverter : IEventConverter<BotGroupFileUploadEvent, GroupFileUploadEventConverter.Data>
{
    public string Name => "group_file_upload";

    public bool CanConvert(BotGroupFileUploadEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotGroupFileUploadEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        GroupId = @event.GroupUin,
        UserId = @event.SenderUin,
        FileId = @event.FileId,
        FileName = @event.FileName,
        FileSize = (long)@event.FileSize,
    });

    public class Data
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
        [JsonPropertyName("file_id")] public required string FileId { get; init; }
        [JsonPropertyName("file_name")] public required string FileName { get; init; }
        [JsonPropertyName("file_size")] public required long FileSize { get; init; }
    }
}
