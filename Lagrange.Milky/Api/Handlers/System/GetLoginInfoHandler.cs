using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;

namespace Lagrange.Milky.Api.Handlers.System;

[ApiHandler("get_login_info")]
public sealed class GetLoginInfoHandler(BotContext lagrange) : INoRequestApiHandler<GetLoginInfoHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(CancellationToken ct)
    {
        string nickname = _lagrange.BotInfo?.Name ?? string.Empty;
        try
        {
            var profile = await _lagrange.FetchStranger(_lagrange.BotUin).WaitAsync(ct);
            if (!string.IsNullOrEmpty(profile.Nickname))
            {
                nickname = profile.Nickname;
                if (_lagrange.Keystore.BotInfo != null)
                {
                    _lagrange.Keystore.BotInfo.Name = profile.Nickname;
                }
                else
                {
                    _lagrange.Keystore.BotInfo = new BotInfo((byte)profile.Age, (byte)profile.Gender, profile.Nickname);
                }
            }
        }
        catch
        {
            
        }

        return new MilkyApiResponse<Result>(new Result
        {
            Uin = _lagrange.BotUin,
            Nickname = nickname,
        });
    }

    public sealed class Result
    {
        [JsonPropertyName("uin")] public required long Uin { get; init; }
        [JsonPropertyName("nickname")] public required string Nickname { get; init; }
    }
}
