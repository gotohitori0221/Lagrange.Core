using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x2DC, 21, true)]
internal class GroupEssenceMessageProcessor : MsgPushProcessorBase
{
    internal override ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType, PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: >= 7 }) return ValueTask.FromResult(false);

        var packet = new BinaryPacket(content!.Value.Span);
        packet.Skip(4 + 1); 

        var notify = ProtoHelper.Deserialize<NotifyMessageBody>(packet.ReadBytes(Prefix.Int16 | Prefix.LengthOnly));
        var essence = notify.EssenceMessage;
        if (essence == null) return ValueTask.FromResult(false);

        bool isSet = essence.SetFlag == 1;

        context.EventInvoker.PostEvent(new BotGroupEssenceEvent(
            essence.GroupUin,
            essence.MsgSequence,
            essence.TimeStamp,
            essence.MemberUin,
            essence.MemberNickName,
            essence.OperatorUin,
            essence.OperatorNickName,
            essence.TimeStamp,
            isSet
        ));

        return ValueTask.FromResult(true);
    }
}
