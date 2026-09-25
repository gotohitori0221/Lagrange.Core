using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.GroupMemberDecreaseNotice, true)]
internal class GroupMemberDecreaseProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType, PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: > 0 }) return false;
        var decrease = ProtoHelper.Deserialize<GroupChange>(content.Value.Span);
        var type = (DecreaseType)decrease.Type;

        if (type == DecreaseType.Disband)
        {
            var op = ProtoHelper.Deserialize<OperatorInfo>(decrease.Operator.AsSpan());
            long operatorUin = await context.CacheContext.ResolveUinAsync(op.Operator.Uid);
            await context.CacheContext.GetGroupList(true);

            context.EventInvoker.PostEvent(new BotGroupDisbandEvent(
                decrease.GroupUin,
                operatorUin == 0 ? null : operatorUin
            ));
            return true;
        }

        if (type is DecreaseType.KickSelf or DecreaseType.Exit or DecreaseType.Kick)
        {
            long memberUin = await context.CacheContext.ResolveGroupMemberUinAsync(decrease.GroupUin, decrease.MemberUid);
            long operatorUin = 0;
            if (type != DecreaseType.Exit)
            {
                var op = ProtoHelper.Deserialize<OperatorInfo>(decrease.Operator.AsSpan());
                operatorUin = await context.CacheContext.ResolveUinAsync(op.Operator.Uid);
            }

            await context.CacheContext.GetMemberList(decrease.GroupUin, true);
            context.EventInvoker.PostEvent(new BotGroupMemberDecreaseEvent(
                decrease.GroupUin,
                memberUin,
                operatorUin == 0 ? null : operatorUin
            ));
            return true;
        }

        context.LogDebug(nameof(PushLogic), "Unknown decrease type: {0}", null, decrease.Type);
        return false;
    }

    private enum DecreaseType
    {
        Disband = 129,
        KickSelf = 3,
        Exit = 130,
        Kick = 131
    }
}