using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.System;

[ApiHandler("get_cookies")]
public sealed class GetCookiesHandler(BotContext lagrange) : IApiHandler<GetCookiesHandler.Request, GetCookiesHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(Request request, CancellationToken ct)
    {
        var cookies = await _lagrange.FetchCookies([request.Domain]).WaitAsync(ct);
        if (!cookies.TryGetValue(request.Domain, out var cookie))
            return new MilkyApiResponse<Result>(-400, $"No cookies for domain: {request.Domain}");

        return new MilkyApiResponse<Result>(new Result { Cookies = cookie });
    }

    public sealed class Request
    {
        [JsonPropertyName("domain")] public required string Domain { get; init; }
    }

    public sealed class Result
    {
        [JsonPropertyName("cookies")] public required string Cookies { get; init; }
    }
}
