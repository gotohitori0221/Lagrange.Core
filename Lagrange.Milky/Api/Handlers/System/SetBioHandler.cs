using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;
using Lagrange.Milky.Serialization;

namespace Lagrange.Milky.Api.Handlers.System;

[ApiHandler("set_bio")]
public sealed class SetBioHandler(BotContext lagrange) : INoResultApiHandler<SetBioHandler.Request>
{
    private readonly BotContext _lagrange = lagrange;

    private static readonly HttpClient _http = new();

    public async ValueTask<MilkyApiResponse> HandleAsync(Request request, CancellationToken ct)
    {
        var cookies = await _lagrange.FetchCookies(["user.qzone.qq.com"]).WaitAsync(ct);
        if (!cookies.TryGetValue("user.qzone.qq.com", out var cookieStr) || string.IsNullOrEmpty(cookieStr))
            return new MilkyApiResponse(-400, "Failed to get cookies for user.qzone.qq.com");

        int bkn = CalcBkn(cookieStr);

        string url = $"https://user.qzone.qq.com/proxy/domain/base.qzone.qq.com/cgi-bin/baseinfo/set_sign?g_tk={bkn}";

        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["sign"] = request.Bio,
        });

        var httpReq = new HttpRequestMessage(HttpMethod.Post, url) { Content = form };
        httpReq.Headers.TryAddWithoutValidation("Cookie", cookieStr);

        var httpResp = await _http.SendAsync(httpReq, ct);
        var raw = await httpResp.Content.ReadAsStringAsync(ct);

        var body = Serializer.JsonDeserialize<QzoneResponse>(raw);
        if (body?.Code != 0)
            return new MilkyApiResponse(-500, $"Failed to set bio, response: {raw}");

        return new MilkyApiResponse();
    }

    private static int CalcBkn(string cookieStr)
    {
        string? token = ExtractCookie(cookieStr, "p_skey") ?? ExtractCookie(cookieStr, "skey");
        if (string.IsNullOrEmpty(token)) return 0;
        long hash = 5381;
        foreach (char c in token) hash += (hash << 5) + c;
        return (int)(hash & 0x7FFFFFFF);
    }

    private static string? ExtractCookie(string cookieStr, string name)
    {
        foreach (var part in cookieStr.Split(';'))
        {
            var kv = part.Trim().Split('=', 2);
            if (kv.Length == 2 && kv[0].Trim() == name) return kv[1].Trim();
        }
        return null;
    }

    public sealed class Request
    {
        [JsonPropertyName("new_bio")] public required string Bio { get; init; }
    }

    public sealed class QzoneResponse
    {
        [JsonPropertyName("code")] public int Code { get; init; }
    }
}
