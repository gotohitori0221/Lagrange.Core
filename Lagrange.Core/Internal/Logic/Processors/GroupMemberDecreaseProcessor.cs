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
        if (content is not { Length: > 0 })
        {
            context.LogWarning(nameof(PushLogic), "Received GroupMemberDecreaseNotice without content, skipped");
            return false;
        }

        var decrease = ProtoHelper.Deserialize<GroupChange>(content.Value.Span);
        var type = (DecreaseType)decrease.Type;

        if (type == DecreaseType.Disband)
        {
            long operatorUin = 0;
            try
            {
                var op = ProtoHelper.Deserialize<OperatorInfo>(decrease.Operator.AsSpan());
                operatorUin = await context.CacheContext.ResolveUinAsync(op.Operator.Uid);
            }
            catch (Exception e)
            {
                context.LogWarning(nameof(PushLogic), "Failed to resolve disband operator: {0}", e, decrease.GroupUin);
            }

            await RefreshGroups(context);

            context.EventInvoker.PostEvent(new BotGroupDisbandEvent(
                decrease.GroupUin,
                operatorUin == 0 ? null : operatorUin
            ));
            return true;
        }

        if (type is DecreaseType.KickSelf or DecreaseType.Exit or DecreaseType.Kick)
        {
            long memberUin = await ResolveMemberUin(context, decrease);
            long operatorUin = 0;
            if (type != DecreaseType.Exit)
            {
                try
                {
                    var op = ProtoHelper.Deserialize<OperatorInfo>(decrease.Operator.AsSpan());
                    operatorUin = await context.CacheContext.ResolveUinAsync(op.Operator.Uid);
                }
                catch (Exception e)
                {
                    context.LogWarning(nameof(PushLogic), "Failed to resolve decrease operator: {0}", e, decrease.GroupUin);
                }
            }

            await RefreshMembers(context, decrease.GroupUin);

            context.EventInvoker.PostEvent(new BotGroupMemberDecreaseEvent(
                decrease.GroupUin,
                memberUin,
                operatorUin == 0 ? null : operatorUin
            ));
            return true;
        }

        context.LogWarning(nameof(PushLogic), "Unknown decrease type: {0}, GroupUin={1}, MemberUid={2}", null, decrease.Type, decrease.GroupUin, decrease.MemberUid);
        return false;
    }

    private static async Task<long> ResolveMemberUin(BotContext context, GroupChange decrease)
    {
        if (string.IsNullOrEmpty(decrease.MemberUid)) return 0;
        // 自己退群/被踢出时该群已不在列表中，拉群成员必然失败，直接用自己的 Uin
        if (decrease.MemberUid == context.Keystore.Uid) return context.BotUin;

        try
        {
            return await context.CacheContext.ResolveGroupMemberUinAsync(decrease.GroupUin, decrease.MemberUid);
        }
        catch (Exception e)
        {
            context.LogWarning(nameof(PushLogic), "Failed to resolve decrease member uid: {0}", e, decrease.MemberUid);
            return 0;
        }
    }

    private static async Task RefreshMembers(BotContext context, long groupUin)
    {
        try
        {
            await context.CacheContext.GetMemberList(groupUin, true);
        }
        catch (Exception e)
        {
            // 群已解散/自己已退出时刷新必然失败，忽略即可，不能因此丢掉事件
            context.LogDebug(nameof(PushLogic), "Skip refresh members of group {0}: {1}", groupUin, e.Message);
        }
    }

    private static async Task RefreshGroups(BotContext context)
    {
        try
        {
            await context.CacheContext.GetGroupList(true);
        }
        catch (Exception e)
        {
            context.LogDebug(nameof(PushLogic), "Skip refresh group list: {0}", e.Message);
        }
    }

    private enum DecreaseType
    {
        Disband = 129,
        KickSelf = 3,
        Exit = 130,
        Kick = 131
    }
}
