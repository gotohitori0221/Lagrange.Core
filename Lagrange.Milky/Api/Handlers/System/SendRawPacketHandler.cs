using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.System;

[ApiHandler("send_raw_packet")]
public sealed class SendRawPacketHandler(BotContext lagrange) : IApiHandler<SendRawPacketHandler.Request, SendRawPacketHandler.Result>
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

        var packet = new BotSsoPacket(request.Command, body);
        var response = await _lagrange
            .SendPacket(packet, (RequestType)request.RequestType, (EncryptType)request.EncryptType)
            .AsTask()
            .WaitAsync(ct);

        return new MilkyApiResponse<Result>(new Result
        {
            Command = request.Command,
            RetCode = response.RetCode,
            RawHex = Convert.ToHexString(response.Data.Span).ToLowerInvariant(),
            RawLength = response.Data.Length,
        });
    }

    public sealed class Request
    {
        [JsonPropertyName("command")] public required string Command { get; init; }

        [JsonPropertyName("body_hex")] public required string BodyHex { get; init; }

        [JsonPropertyName("request_type")] public int RequestType { get; init; } = (int)Lagrange.Core.Common.Entity.RequestType.D2Auth;

        [JsonPropertyName("encrypt_type")] public int EncryptType { get; init; } = (int)Lagrange.Core.Common.Entity.EncryptType.EncryptD2Key;
    }

    public sealed class Result
    {
        [JsonPropertyName("command")] public required string Command { get; init; }

        [JsonPropertyName("retcode")] public required int RetCode { get; init; }

        [JsonPropertyName("raw_hex")] public required string RawHex { get; init; }

        [JsonPropertyName("raw_length")] public required int RawLength { get; init; }
    }
}
