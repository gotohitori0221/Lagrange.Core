using Lagrange.Core.Common.Entity;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x20D, true)]
internal class GroupInviteProcessor : MsgPushProcessorBase
{
    private static readonly TimeSpan[] RetryDelays = [
        TimeSpan.Zero,
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromMilliseconds(300),
        TimeSpan.FromMilliseconds(800),
    ];

    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType,
        PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: > 0 }) return false;
        var @event = ProtoHelper.Deserialize<Event0x20D>(content.Value.Span);
        if (@event.SubType != 87) return false;

        var body = ProtoHelper.Deserialize<GroupInvite>(@event.Body);
        foreach (var delay in RetryDelays)
        {
            if (delay > TimeSpan.Zero) await Task.Delay(delay);
            var notification = await FindNotification(context, body);
            if (notification == null) continue;

            context.EventInvoker.PostEvent(new BotGroupInviteNotificationEvent(notification));
            return true;
        }

        context.LogWarning(nameof(PushLogic), "Received GroupInviteNotification but no corresponding notification found");
        return false;
    }

    private static async Task<BotGroupInviteNotification?> FindNotification(BotContext context, GroupInvite body)
    {
        var normalTask = context.EventContext
            .SendEvent<FetchGroupNotificationsEventResp>(new FetchGroupNotificationsEventReq(30)).AsTask();
        var filteredTask = context.EventContext
            .SendEvent<FetchFilteredGroupNotificationsEventResp>(new FetchFilteredGroupNotificationsEventReq(10)).AsTask();
        await Task.WhenAll(normalTask, filteredTask);

        return (await normalTask).GroupNotifications
            .Concat((await filteredTask).GroupNotifications)
            .OfType<BotGroupInviteNotification>()
            .FirstOrDefault(notification =>
                body.Body.GroupUin == notification.GroupUin &&
                body.Body.InviterUid == notification.InviterUid &&
                body.Body.TargetUid == notification.TargetUid &&
                notification.State == BotGroupNotificationState.Wait);
    }
}