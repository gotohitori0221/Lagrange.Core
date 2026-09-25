using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;
using Lagrange.Milky.Converters;

namespace Lagrange.Milky.Api.Handlers.Group;

[ApiHandler("set_group_avatar")]
public sealed class SetGroupAvatarHandler(BotContext lagrange, ResourceConverter resourceConverter) : INoResultApiHandler<SetGroupAvatarHandler.Request>
{
    private readonly BotContext _lagrange = lagrange;
    private readonly ResourceConverter _resourceConverter = resourceConverter;

    public async ValueTask<MilkyApiResponse> HandleAsync(Request request, CancellationToken ct)
    {
        using var stream = await _resourceConverter.UriToStreamAsync(request.Uri, ct);
        return await _lagrange.SetGroupAvatar(request.GroupId, stream).WaitAsync(ct)
            ? new MilkyApiResponse()
            : new MilkyApiResponse(-500, "unknown error");
    }

    public sealed class Request
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("image_uri")] public required string Uri { get; init; }
    }
}
