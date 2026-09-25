using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x210, 39, true)] 
internal class PinChangedProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType,
        PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        var body = ProtoHelper.Deserialize<FriendDeleteOrPinChanged>(content!.Value.Span);

        return body.Body.Type switch
        {
            5 => await HandleFriendDelete(context, body),
            7 => await HandlePinChanged(context, body),
            _ => false,
        };
    }

    private async ValueTask<bool> HandleFriendDelete(BotContext context, FriendDeleteOrPinChanged body)
    {
        _ = await context.CacheContext.GetFriendList(true); 
        return true;
    }

    private static async ValueTask<bool> HandlePinChanged(BotContext context, FriendDeleteOrPinChanged body)
    {
        if (body.Body.PinChanged == null) return false;

        var pinChanged = body.Body.PinChanged;
        long uin = await context.CacheContext.ResolveUinAsync(pinChanged.Body.Uid);
        long? groupUin = pinChanged.Body.GroupUin;

        bool isPin = pinChanged.Body.Info.Timestamp.Length != 0;

        context.EventInvoker.PostEvent(new BotPinChangedEvent(uin, groupUin, isPin));
        return true;
    }
}