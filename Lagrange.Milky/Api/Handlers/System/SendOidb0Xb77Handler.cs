using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.System;

[ApiHandler("send_oidb_0xb77")]
public sealed class SendOidb0Xb77Handler(BotContext lagrange) : IApiHandler<SendOidb0Xb77Handler.Request, SendOidb0Xb77Handler.Result>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(Request request, CancellationToken ct)
    {
        byte[] body;
        try
        {
            body = Convert.FromHexString(request.BodyHex);
        }
        catch (FormatException)
        {
            return new MilkyApiResponse<Result>(Constants.RetcodeInvalidParameter, "body_hex is not a valid hex string");
        }

        var (result, message, responseBody) = await _lagrange.SendOidb0Xb77(body).WaitAsync(ct);

        return new MilkyApiResponse<Result>(new Result
        {
            OidbResult = result,
            Message = message,
            BodyHex = Convert.ToHexString(responseBody.Span).ToLowerInvariant(),
        });
    }

    public sealed class Request(string bodyHex)
    {
        [JsonPropertyName("body_hex")] public required string BodyHex { get; init; } = bodyHex;
    }

    public sealed class Result
    {
        [JsonPropertyName("result")] public required uint OidbResult { get; init; }

        [JsonPropertyName("message")] public required string Message { get; init; }

        [JsonPropertyName("body_hex")] public required string BodyHex { get; init; }
    }
}
