using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x2DC, 17, true)]
internal class GroupRecallMessageProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType, PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: >= 7 }) return false;

        var packet = new BinaryPacket(content.Value.Span);
        packet.Skip(4 + 1);

        var notify = ProtoHelper.Deserialize<NotifyMessageBody>(packet.ReadBytes(Prefix.Int16 | Prefix.LengthOnly));
        long operatorUin = await context.CacheContext.ResolveGroupMemberUinAsync(notify.GroupUin, notify.Recall.OperatorUid);

        foreach (var message in notify.Recall.RecallMessages)
        {
            long authorUin = await context.CacheContext.ResolveGroupMemberUinAsync(notify.GroupUin, message.AuthorUid);
            context.EventInvoker.PostEvent(new BotGroupRecallEvent(
                notify.GroupUin,
                message.Sequence,
                authorUin,
                operatorUin,
                notify.Recall.TipInfo?.Tip ?? string.Empty
            ));
        }

        return true;
    }
}