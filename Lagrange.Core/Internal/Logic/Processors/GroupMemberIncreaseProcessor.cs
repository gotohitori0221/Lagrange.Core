using System.Text;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.GroupMemberIncreaseNotice, true)]
internal class GroupMemberIncreaseProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType, PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: > 0 }) return false;

        var increase = ProtoHelper.Deserialize<GroupChange>(content.Value.Span);
        if (increase.Type is not (130 or 131)) return false;

        string? operatorUid = increase.Operator is { Length: > 0 }
            ? Encoding.UTF8.GetString(increase.Operator)
            : null;

        await context.CacheContext.GetMemberList(increase.GroupUin, true);

        long memberUin = await context.CacheContext.ResolveUinAsync(increase.MemberUid);
        long operatorUin = await context.CacheContext.ResolveUinAsync(operatorUid);

        var @event = increase.Type == 130
            ? new BotGroupMemberIncreaseEvent(increase.GroupUin, memberUin, 0, increase.Type, operatorUin)
            : new BotGroupMemberIncreaseEvent(increase.GroupUin, memberUin, operatorUin, increase.Type, 0);

        context.EventInvoker.PostEvent(@event);
        return true;
    }
}