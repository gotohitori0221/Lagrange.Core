using System;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x2DC, 12, true)]
internal class GroupMuteProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType,
        PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: > 0 }) return false;
        var body = ProtoHelper.Deserialize<GroupMuteBody>(content.Value.Span);
        var state = body.Info?.State;
        long operatorUin = await context.CacheContext.ResolveUinAsync(body.OperatorUid);

        if (state is { TargetUid.Length: > 0 })
        {
            long targetUin = await context.CacheContext.ResolveUinAsync(state.TargetUid);
            context.EventInvoker.PostEvent(new BotGroupMuteEvent(
                (long)body.GroupUin,
                operatorUin,
                targetUin,
                state.Duration
            ));

            if (targetUin != 0)
            {
                long shutUpTimestamp = state.Duration == 0
                    ? 0
                    : DateTimeOffset.UtcNow.ToUnixTimeSeconds() + state.Duration;
                context.CacheContext.UpdateMemberShutUp((long)body.GroupUin, targetUin, shutUpTimestamp);
            }
        }
        else
        {
            uint duration = state?.Duration ?? 0;
            context.EventInvoker.PostEvent(new BotGroupWholeMuteEvent(
                (long)body.GroupUin,
                operatorUin,
                duration > 0
            ));
        }

        return true;
    }
}