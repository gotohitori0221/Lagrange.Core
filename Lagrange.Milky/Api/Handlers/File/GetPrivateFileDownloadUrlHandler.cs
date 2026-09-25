using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.File;

[ApiHandler("get_private_file_download_url")]
public sealed class GetPrivateFileDownloadUrlHandler(BotContext lagrange) : IApiHandler<GetPrivateFileDownloadUrlHandler.Request, GetPrivateFileDownloadUrlHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(Request request, CancellationToken ct)
    {
        var url = await _lagrange.GetNTV2RichMediaUrl(request.FileId).WaitAsync(ct);
        return new MilkyApiResponse<Result>(new Result { DownloadUrl = url });
    }

    public sealed class Request
    {
        [JsonPropertyName("user_id")] public long UserId { get; init; }
        [JsonPropertyName("file_id")] public required string FileId { get; init; }
        [JsonPropertyName("file_hash")] public string? FileHash { get; init; }
        [JsonPropertyName("is_self_send")] public bool IsSelfSend { get; init; }
    }

    public sealed class Result
    {
        [JsonPropertyName("download_url")] public required string DownloadUrl { get; init; }
    }
}
