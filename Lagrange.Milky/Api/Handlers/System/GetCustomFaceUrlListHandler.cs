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

[ApiHandler("get_custom_face_url_list")]
public sealed class GetCustomFaceUrlListHandler(BotContext lagrange) : INoRequestApiHandler<GetCustomFaceUrlListHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;

    private static readonly HttpClient _http = new();

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(CancellationToken ct)
    {
        long userId = _lagrange.BotUin;

        var cookies = await _lagrange.FetchCookies(["face.qq.com"]).WaitAsync(ct);
        if (!cookies.TryGetValue("face.qq.com", out var cookieStr) || string.IsNullOrEmpty(cookieStr))
            return new MilkyApiResponse<Result>(new Result { Urls = [] });

        int bkn = CalcBkn(cookieStr);

        string url = $"https://faces.qzone.qq.com/cgi-bin/face/facelist?uin={userId}&bkn={bkn}";

        var httpReq = new HttpRequestMessage(HttpMethod.Get, url);
        httpReq.Headers.TryAddWithoutValidation("Cookie", cookieStr);

        HttpResponseMessage httpResp;
        string raw;
        try
        {
            httpResp = await _http.SendAsync(httpReq, ct);
            raw = await httpResp.Content.ReadAsStringAsync(ct);
        }
        catch
        {
            return new MilkyApiResponse<Result>(new Result { Urls = [] });
        }

        var body = Serializer.JsonDeserialize<FaceListResponse>(raw);
        if (body?.Urls == null)
            return new MilkyApiResponse<Result>(new Result { Urls = [] });

        return new MilkyApiResponse<Result>(new Result { Urls = body.Urls });
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

    public sealed class Result
    {
        [JsonPropertyName("urls")] public required List<string> Urls { get; init; }
    }

    public sealed class FaceListResponse
    {
        [JsonPropertyName("face_urls")] public List<string>? Urls { get; init; }
    }
}
