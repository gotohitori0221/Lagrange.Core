using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;
using Lagrange.Milky.Serialization;

namespace Lagrange.Milky.Api.Handlers.Group;

[ApiHandler("get_group_announcements")]
public sealed class GetGroupAnnouncementsHandler(BotContext lagrange) : IApiHandler<GetGroupAnnouncementsHandler.Request, GetGroupAnnouncementsHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;

    private static readonly HttpClient _http = new();

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(Request request, CancellationToken ct)
    {
        var cookies = await _lagrange.FetchCookies(["qun.qq.com"]).WaitAsync(ct);
        if (!cookies.TryGetValue("qun.qq.com", out var cookieStr) || string.IsNullOrEmpty(cookieStr))
            return new MilkyApiResponse<Result>(-400, "Failed to get cookies for qun.qq.com");

        int bkn = CalcBkn(cookieStr);
        int pageSize = 10;

        string url = $"https://web.qun.qq.com/cgi-bin/announce/get_t_list?bkn={bkn}&qid={request.GroupId}&ni=1&n=1&i=1&log_and_redirect=1&s=-1&e={pageSize}";

        var httpReq = new HttpRequestMessage(HttpMethod.Get, url);
        httpReq.Headers.TryAddWithoutValidation("Cookie", cookieStr);
        var httpResp = await _http.SendAsync(httpReq, ct);
        var raw = await httpResp.Content.ReadAsStringAsync(ct);

        var body = Serializer.JsonDeserialize<GetAnnouncementsResponse>(raw);
        if (body == null)
            return new MilkyApiResponse<Result>(-500, "Failed to parse group announcement list response");

        if (body.Code != 0)
            return new MilkyApiResponse<Result>(-400, $"API error code: {body.Code}");

        var announcements = (body.Items ?? []).Select(item => new GroupAnnouncement
        {
            GroupId = request.GroupId,
            AnnouncementId = item.Fid,
            UserId = long.TryParse(item.Uin, out var uin) ? uin : 0,
            Time = item.PublishTime,
            Content = item.Text,
            ImageUrl = null,
        }).ToList();

        return new MilkyApiResponse<Result>(new Result { Announcements = announcements });
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
    }

    public sealed class Result
    {
        [JsonPropertyName("announcements")] public required List<GroupAnnouncement> Announcements { get; init; }
    }

    public sealed class GroupAnnouncement
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("announcement_id")] public required string AnnouncementId { get; init; }
        [JsonPropertyName("user_id")] public required long UserId { get; init; }
        [JsonPropertyName("time")] public required long Time { get; init; }
        [JsonPropertyName("content")] public required string Content { get; init; }
        [JsonPropertyName("image_url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ImageUrl { get; init; }
    }

    public sealed class GetAnnouncementsResponse
    {
        [JsonPropertyName("ec")] public int Code { get; init; }
        [JsonPropertyName("mn")] public List<AnnouncementItem>? Items { get; init; }
    }

    public sealed class AnnouncementItem
    {
        [JsonPropertyName("fid")] public string Fid { get; init; } = "";
        [JsonPropertyName("u")] public string Uin { get; init; } = "";
        [JsonPropertyName("n")] public string Nick { get; init; } = "";
        [JsonPropertyName("text")] public string Text { get; init; } = "";
        [JsonPropertyName("pubt")] public long PublishTime { get; init; }
    }
}
