using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x210, 138, true)]
[MsgPushProcessor(MsgType.Event0x210, 139, true)]
internal class FriendRecallMessageProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext bot, MsgType msgType, int subType, PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: > 0 }) return false;
        var recall = ProtoHelper.Deserialize<FriendRecall>(content.Value.Span);

        var uins = await Task.WhenAll(
            bot.CacheContext.ResolveUinAsync(recall.Info.FromUid),
            bot.CacheContext.ResolveUinAsync(recall.Info.ToUid)
        );
        long fromUin = uins[0];
        long toUin = uins[1];

        bot.EventInvoker.PostEvent(new BotFriendRecallEvent(
            fromUin == bot.BotUin ? toUin : fromUin,
            fromUin,
            (ulong)recall.Info.Sequence,
            recall.Info.TipInfo?.Tip ?? string.Empty
        ));

        return true;
    }
}