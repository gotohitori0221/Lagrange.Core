using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using Lagrange.Milky.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Common.Interface;
using Lagrange.Milky.Api.Attributes;
using Lagrange.Milky.Converters;
using Lagrange.Milky.Models.Segments;
using Lagrange.Core.Message;

namespace Lagrange.Milky.Api.Handlers.Group;

[ApiHandler("get_group_essence_messages")]
public sealed class GetGroupEssenceMessagesHandler(BotContext lagrange, MilkyConverter converter) : IApiHandler<GetGroupEssenceMessagesHandler.Request, GetGroupEssenceMessagesHandler.Result>
{
    private readonly BotContext _lagrange = lagrange;
    private readonly MilkyConverter _converter = converter;

    private static readonly HttpClient _http = new();

    public async ValueTask<MilkyApiResponse<Result>> HandleAsync(Request request, CancellationToken ct)
    {
        
        var cookies = await _lagrange.FetchCookies(["qun.qq.com"]).WaitAsync(ct);
        if (!cookies.TryGetValue("qun.qq.com", out var cookieStr) || string.IsNullOrEmpty(cookieStr))
            return new MilkyApiResponse<Result>(-400, "Failed to get cookies for qun.qq.com");

        int bkn = CalcBkn(cookieStr);

        string url = $"https://qun.qq.com/cgi-bin/group_digest/digest_list?random=7800&X-CROSS-ORIGIN=fetch" +
                     $"&group_code={request.GroupId}&page_start={request.PageIndex}&page_limit={request.PageSize}&bkn={bkn}";

        var httpReq = new HttpRequestMessage(HttpMethod.Get, url);
        httpReq.Headers.TryAddWithoutValidation("Cookie", cookieStr);
        var httpResp = await _http.SendAsync(httpReq, ct);
        var raw = await httpResp.Content.ReadAsStringAsync(ct);

        var body = Serializer.JsonDeserialize<EssenceListResponse>(raw);
        if (body?.Data == null)
            return new MilkyApiResponse<Result>(-500, "Failed to parse essence message list response");

        var result = new List<GroupEssenceMessage>();
        foreach (var msg in body.Data.MsgList ?? [])
        {
            
            var messages = await _lagrange.GetGroupMessage(request.GroupId, msg.MsgSeq, msg.MsgSeq).WaitAsync(ct);
            var botMessage = messages.FirstOrDefault();

            IReadOnlyList<IncomingSegmentBase> segments;
            if (botMessage != null)
            {
                segments = await _converter.ToIncomingSegmentsAsync(botMessage.Entities, botMessage.Type, request.GroupId, ct);
            }
            else
            {
                segments = [];
            }

            result.Add(new GroupEssenceMessage
            {
                GroupId = request.GroupId,
                MessageSeq = (long)msg.MsgSeq,
                MessageTime = msg.SenderTime,
                SenderId = long.TryParse(msg.SenderUin, out var senderUin) ? senderUin : 0,
                SenderName = msg.SenderNick,
                OperatorId = long.TryParse(msg.AddDigestUin, out var opUin) ? opUin : 0,
                OperatorName = msg.AddDigestNick,
                OperationTime = msg.AddDigestTime,
                Segments = segments,
            });
        }

        return new MilkyApiResponse<Result>(new Result
        {
            Messages = result,
            IsEnd = body.Data.IsEnd,
        });
    }

    
    private static int CalcBkn(string cookieStr)
    {
        
        string? token = ExtractCookie(cookieStr, "p_skey") ?? ExtractCookie(cookieStr, "skey");
        if (string.IsNullOrEmpty(token)) return 0;

        long hash = 5381;
        foreach (char c in token)
        {
            hash += (hash << 5) + c;
        }
        return (int)(hash & 0x7FFFFFFF);
    }

    private static string? ExtractCookie(string cookieStr, string name)
    {
        foreach (var part in cookieStr.Split(';'))
        {
            var kv = part.Trim().Split('=', 2);
            if (kv.Length == 2 && kv[0].Trim() == name)
                return kv[1].Trim();
        }
        return null;
    }

    public sealed class Request
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("page_index")] public int PageIndex { get; init; } = 0;
        [JsonPropertyName("page_size")] public int PageSize { get; init; } = 20;
    }

    public sealed class Result
    {
        [JsonPropertyName("messages")] public required IReadOnlyList<GroupEssenceMessage> Messages { get; init; }
        [JsonPropertyName("is_end")] public required bool IsEnd { get; init; }
    }

    public sealed class GroupEssenceMessage
    {
        [JsonPropertyName("group_id")] public required long GroupId { get; init; }
        [JsonPropertyName("message_seq")] public required long MessageSeq { get; init; }
        [JsonPropertyName("message_time")] public required long MessageTime { get; init; }
        [JsonPropertyName("sender_id")] public required long SenderId { get; init; }
        [JsonPropertyName("sender_name")] public required string SenderName { get; init; }
        [JsonPropertyName("operator_id")] public required long OperatorId { get; init; }
        [JsonPropertyName("operator_name")] public required string OperatorName { get; init; }
        [JsonPropertyName("operation_time")] public required long OperationTime { get; init; }
        [JsonPropertyName("segments")] public required IReadOnlyList<IncomingSegmentBase> Segments { get; init; }
    }

    
    public sealed class EssenceListResponse
    {
        [JsonPropertyName("retcode")] public long Retcode { get; init; }
        [JsonPropertyName("data")] public EssenceListData? Data { get; init; }
    }

    public sealed class EssenceListData
    {
        [JsonPropertyName("msg_list")] public List<EssenceMsgItem>? MsgList { get; init; }
        [JsonPropertyName("is_end")] public bool IsEnd { get; init; }
    }

    public sealed class EssenceMsgItem
    {
        [JsonPropertyName("group_code")] public string GroupCode { get; init; } = "";
        [JsonPropertyName("msg_seq")] public ulong MsgSeq { get; init; }
        [JsonPropertyName("msg_random")] public uint MsgRandom { get; init; }
        [JsonPropertyName("sender_uin")] public string SenderUin { get; init; } = "";
        [JsonPropertyName("sender_nick")] public string SenderNick { get; init; } = "";
        [JsonPropertyName("sender_time")] public uint SenderTime { get; init; }
        [JsonPropertyName("add_digest_uin")] public string AddDigestUin { get; init; } = "";
        [JsonPropertyName("add_digest_nick")] public string AddDigestNick { get; init; } = "";
        [JsonPropertyName("add_digest_time")] public uint AddDigestTime { get; init; }
    }
}
