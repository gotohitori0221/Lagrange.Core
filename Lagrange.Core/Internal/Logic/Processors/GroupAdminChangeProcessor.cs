using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x2DC, 14, true)] 
[MsgPushProcessor(MsgType.Event0x2DC, 15, true)] 
internal class GroupAdminChangeProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType,
        PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: >= 7 }) return false;

        var packet = new BinaryPacket(content.Value.Span);
        packet.Skip(4 + 1);

        var proto = packet.ReadBytes(Prefix.Int16 | Prefix.LengthOnly);
        var body = ProtoHelper.Deserialize<GroupAdminChangeBody>(proto);

        long targetUin = await context.CacheContext.ResolveGroupMemberUinAsync((long)body.GroupUin, body.TargetUid);
        long operatorUin = msgEvt.MsgPush.CommonMessage.RoutingHead.FromUin;

        context.EventInvoker.PostEvent(new BotGroupAdminChangeEvent(
            (long)body.GroupUin,
            targetUin,
            body.IsAdmin != 0,
            operatorUin
        ));

        return true;
    }
}
