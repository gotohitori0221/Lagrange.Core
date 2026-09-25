using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x2DC, 16, true)] 
internal class GroupNameChangeProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType,
        PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: >= 7 }) return false;

        var packet = new BinaryPacket(content.Value.Span);
        packet.Skip(4 + 1);

        var proto = packet.ReadBytes(Prefix.Int16 | Prefix.LengthOnly);
        var notify = ProtoHelper.Deserialize<NotifyMessageBody>(proto);
        if (notify.SubType != 12 || notify.EventParam is not { Length: > 0 }) return false;

        var body = ProtoHelper.Deserialize<GroupNameChangeBody>(new ReadOnlySpan<byte>(notify.EventParam));

        long operatorUin = await context.CacheContext.ResolveGroupMemberUinAsync(notify.GroupUin, notify.OperatorUid);

        context.EventInvoker.PostEvent(new BotGroupNameChangeEvent(
            notify.GroupUin,
            string.Empty,
            body.NewName ?? string.Empty,
            operatorUin
        ));

        return true;
    }
}
