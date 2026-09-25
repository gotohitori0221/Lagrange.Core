using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x210, 290, true)]
[MsgPushProcessor(MsgType.Event0x210, 291, true)]
internal class FriendNudgeProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType,
        PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: > 0 }) return false;
        var payload = content.Value.Span;

        if (TryParseGrayTip(payload, out var grayTip))
        {
            var parameters = grayTip.MsgTemplParam.ToDictionary(x => x.Name, x => x.Value);
            if (parameters.TryGetValue("uin_str1", out var senderText) && long.TryParse(senderText, out long senderUin) &&
                parameters.TryGetValue("uin_str2", out var receiverText) && long.TryParse(receiverText, out long receiverUin))
            {
                long selfUin = context.BotUin;
                string action = parameters.GetValueOrDefault("action_str")
                    ?? parameters.GetValueOrDefault("alt_str1")
                    ?? string.Empty;

                context.EventInvoker.PostEvent(new BotFriendNudgeEvent(
                    senderUin == selfUin ? receiverUin : senderUin,
                    senderUin,
                    senderUin == selfUin,
                    receiverUin == selfUin,
                    action,
                    parameters.GetValueOrDefault("action_img_url") ?? string.Empty,
                    parameters.GetValueOrDefault("suffix_str") ?? string.Empty
                ));
                return true;
            }
        }

        var pokeInfo = ProtoHelper.Deserialize<FriendRecallPokeInfo>(payload);
        if (string.IsNullOrEmpty(pokeInfo.PeerUid) || string.IsNullOrEmpty(pokeInfo.OperatorUid)) return false;

        long self = context.BotUin;
        var resolvedUins = await Task.WhenAll(
            context.CacheContext.ResolveUinAsync(pokeInfo.PeerUid),
            context.CacheContext.ResolveUinAsync(pokeInfo.OperatorUid)
        );
        long peerUin = resolvedUins[0];
        long operatorUin = resolvedUins[1];

        context.EventInvoker.PostEvent(new BotFriendNudgeEvent(
            operatorUin == self ? peerUin : operatorUin,
            operatorUin,
            operatorUin == self,
            peerUin == self,
            string.Empty,
            string.Empty,
            string.Empty
        ));
        return true;
    }

    private static bool TryParseGrayTip(ReadOnlySpan<byte> payload, out GeneralGrayTipInfo grayTip)
    {
        try
        {
            var candidate = ProtoHelper.Deserialize<GeneralGrayTipInfo>(payload);
            if (candidate.BusiType == 12 && candidate.MsgTemplParam is { Count: > 0 })
            {
                grayTip = candidate;
                return true;
            }
        }
        catch { }

        grayTip = null!;
        return false;
    }
}