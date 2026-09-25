using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;
using Lagrange.Milky.Serialization;

namespace Lagrange.Milky.Api.Handlers.Group;

[ApiHandler("delete_group_announcement")]
public sealed class DeleteGroupAnnouncementHandler(BotContext lagrange) : INoResultApiHandler<DeleteGroupAnnouncementHandler.Request>
{
    private readonly BotContext _lagrange = lagrange;

    private static readonly HttpClient _http = new();

    public async ValueTask<MilkyApiResponse> HandleAsync(Request request, CancellationToken ct)
    {
        var cookies = await _lagrange.FetchCookies(["qun.qq.com"]).WaitAsync(ct);
        if (!cookies.TryGetValue("qun.qq.com", out var cookieStr) || string.IsNullOrEmpty(cookieStr))
            return new MilkyApiResponse(-400, "Failed to get cookies for qun.qq.com");

        int bkn = CalcBkn(cookieStr);

        var formData = new Dictionary<string, string>
        {
            ["bkn"] = bkn.ToString(),
            ["qid"] = request.GroupId.ToString(),
            ["fid"] = request.AnnouncementId,
        };

        var httpReq = new HttpRequestMessage(HttpMethod.Post, "https://web.qun.qq.com/cgi-bin/announce/del_qun_notice")
        {
            Content = new FormUrlEncodedContent(formData),
        };
        httpReq.Headers.TryAddWithoutValidation("Cookie", cookieStr);
        var httpResp = await _http.SendAsync(httpReq, ct);
        var raw = await httpResp.Content.ReadAsStringAsync(ct);

        var body = Serializer.JsonDeserialize<AnnouncementOperationResponse>(raw);
        if (body == null)
            return new MilkyApiResponse(-500, "Failed to parse delete group announcement response");

        if (body.Code != 0)
            return new MilkyApiResponse(-400, $"API error code: {body.Code}");

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
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("announcement_id")] public required string AnnouncementId { get; init; }
    }

    public sealed class AnnouncementOperationResponse
    {
        [JsonPropertyName("ec")] public int Code { get; init; }
    }
}
