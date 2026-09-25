using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events.Attributes;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public class FriendFileUploadEventConverter : IEventConverter<BotFriendFileUploadEvent, FriendFileUploadEventConverter.Data>
{
    public string Name => "friend_file_upload";

    public bool CanConvert(BotFriendFileUploadEvent @event) => true;

    public ValueTask<Data> ConvertAsync(BotFriendFileUploadEvent @event, CancellationToken ct) => ValueTask.FromResult(new Data
    {
        UserId = @event.SenderUin,
        FileId = @event.FileId,
        FileName = @event.FileName,
        FileSize = (long)@event.FileSize,
        FileHash = @event.FileHash,
        IsSelf = @event.IsSelf,
    });

    public class Data
    {
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
        [JsonPropertyName("file_id")] public required string FileId { get; init; }
        [JsonPropertyName("file_name")] public required string FileName { get; init; }
        [JsonPropertyName("file_size")] public required long FileSize { get; init; }
        [JsonPropertyName("file_hash")] public required string FileHash { get; init; }
        [JsonPropertyName("is_self")] public required bool IsSelf { get; init; }
    }
}
