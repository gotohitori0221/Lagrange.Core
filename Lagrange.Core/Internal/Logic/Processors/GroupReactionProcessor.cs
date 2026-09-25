using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x2DC, 16, true)]
internal class GroupReactionProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType,
        PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: >= 7 }) return false;

        var reader = new BinaryPacket(content.Value.Span);
        reader.Skip(4 + 1);
        var proto = reader.ReadBytes(Prefix.Int16 | Prefix.LengthOnly);
        var body = ProtoHelper.Deserialize<NotifyMessageBody>(proto);
        if (body.SubType != 35) return false;
        var reaction = body.Reaction.Data.Data;

        long @operator = await context.CacheContext.ResolveGroupMemberUinAsync(body.GroupUin, reaction.Data.OperatorUid);

        context.EventInvoker.PostEvent(new BotGroupReactionEvent(
            body.GroupUin,
            reaction.Target.Sequence,
            @operator,
            reaction.Data.Type == 1,
            reaction.Data.Code,
            reaction.Data.CurrentCount,
            reaction.Data.ReactionType
        ));
        return true;
    }
}