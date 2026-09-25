using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.System;

[ApiHandler("get_csrf_token")]
public sealed class GetCsrfTokenHandler(BotContext lagrange) : INoRequestApiHandler<GetCsrfTokenHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(CancellationToken ct)
    {
        var (key, _) = await _lagrange.FetchClientKey().WaitAsync(ct);
        return new MilkyApiResponse<Result>(new Result
        {
            CsrfToken = key,
        });
    }

    public sealed class Result
    {
        [JsonPropertyName("csrf_token")] public required string CsrfToken { get; init; }
    }
}
