using Lagrange.Core.Common.Entity;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.GroupJoinNotification, true)]
internal class GroupJoinProcessor : MsgPushProcessorBase
{
    private static readonly TimeSpan[] RetryDelays = [
        TimeSpan.Zero,
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromMilliseconds(300),
        TimeSpan.FromMilliseconds(800),
    ];

    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType, PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: > 0 }) return false;
        var join = ProtoHelper.Deserialize<GroupJoin>(content.Value.Span);

        foreach (var delay in RetryDelays)
        {
            if (delay > TimeSpan.Zero) await Task.Delay(delay);
            var notification = await FindNotification(context, join);
            if (notification == null) continue;

            context.EventInvoker.PostEvent(new BotGroupJoinNotificationEvent(notification));
            return true;
        }

        context.LogWarning(nameof(PushLogic), "Received GroupJoinNotification but no corresponding notification found");
        return false;
    }

    private static async Task<BotGroupJoinNotification?> FindNotification(BotContext context, GroupJoin join)
    {
        var normalTask = context.EventContext
            .SendEvent<FetchGroupNotificationsEventResp>(new FetchGroupNotificationsEventReq(30)).AsTask();
        var filteredTask = context.EventContext
            .SendEvent<FetchFilteredGroupNotificationsEventResp>(new FetchFilteredGroupNotificationsEventReq(10)).AsTask();
        await Task.WhenAll(normalTask, filteredTask);

        return (await normalTask).GroupNotifications
            .Concat((await filteredTask).GroupNotifications)
            .OfType<BotGroupJoinNotification>()
            .FirstOrDefault(notification =>
                join.GroupUin == notification.GroupUin &&
                join.TargetUid == notification.TargetUid &&
                notification.State == BotGroupNotificationState.Wait);
    }
}