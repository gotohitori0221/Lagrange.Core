using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x20D, 6, true)] 
internal class GroupFileUploadProcessor : MsgPushProcessorBase
{
    internal override ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType,
        PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        var notify = ProtoHelper.Deserialize<GroupFileUploadNotify>(content!.Value.Span);

        if (notify.FileInfo == null) return ValueTask.FromResult(false);

        context.EventInvoker.PostEvent(new BotGroupFileUploadEvent(
            (long)notify.GroupUin,
            (long)notify.SenderUin,
            notify.FileInfo.FileName ?? string.Empty,
            notify.FileInfo.FileSize,
            notify.FileInfo.FileId ?? string.Empty
        ));

        return ValueTask.FromResult(true);
    }
}
