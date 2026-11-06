using System.Collections.Frozen;
using Lagrange.Core.Common;
using Lagrange.Core.Events;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Services;

namespace Lagrange.Core.Internal.Logic;

[EventSubscribe<PushMessageEvent>(Protocols.All)]
internal class PushLogic(BotContext ctx) : ILogic
{
    private readonly FrozenDictionary<MsgMatchKey, List<MsgPushProcessorBase>> _processors = MsgPushProcessorRegistry.Create();

    public async ValueTask Incoming(ProtocolEvent e)
    {
        if (e is not PushMessageEvent msgEvt) return;
        if (msgEvt.MsgPush?.CommonMessage?.ContentHead is null) return;
        var msgType = (MsgType)msgEvt.MsgPush.CommonMessage.ContentHead.Type;
        var subType = msgEvt.MsgPush.CommonMessage.ContentHead.SubType;
        var hasContent = msgEvt.MsgPush.CommonMessage.MessageBody?.MsgContent is not null;
        var content = msgEvt.MsgPush.CommonMessage.MessageBody?.MsgContent;

        if (await Dispatch(msgEvt, msgType, subType, hasContent, content)) return;
        if (await Dispatch(msgEvt, msgType, -1, hasContent, content)) return;

        // 兜底：部分通知的 MsgContent 可能为空，处理器注册的 requireContent 与实际不符时仍应有机会自行判断
        if (await Dispatch(msgEvt, msgType, subType, !hasContent, content)) return;
        if (await Dispatch(msgEvt, msgType, -1, !hasContent, content)) return;
    }

    private async ValueTask<bool> Dispatch(PushMessageEvent msgEvt, MsgType msgType, int subType, bool requireContent, ReadOnlyMemory<byte>? content)
    {
        if (!_processors.TryGetValue(new MsgMatchKey(msgType, subType, requireContent), out var handlers)) return false;

        foreach (var handler in handlers)
        {
            if (await handler.Handle(ctx, msgType, subType, msgEvt, content)) return true;
        }

        return false;
    }
}

internal enum MsgType
{
    GroupMemberIncreaseNotice = 33,
    GroupMemberDecreaseNotice = 34,
    GroupMessage = 82,
    GroupJoinNotification = 84,
    TempMessage = 141,
    PrivateMessage = 166,
    FriendFileMessage = 529,
    Event0x20D = 525,
    Event0x210 = 528,  
    Event0x2DC = 732,  
}

internal readonly record struct MsgMatchKey(MsgType MsgType, int SubType = -1, bool RequireContent = false);

internal abstract class MsgPushProcessorBase
{
    internal abstract ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType, PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content);
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class MsgPushProcessorAttribute : Attribute
{
    public MsgType MsgType { get; init; }
    public int SubType { get; }
    public bool RequireContent { get; } = false;

    public MsgPushProcessorAttribute(MsgType msgType, bool requireContent = false)
        : this(msgType, -1, requireContent) { }

    public MsgPushProcessorAttribute(MsgType msgType, int subType, bool requireContent = false)
    {
        MsgType = msgType;
        SubType = subType;
        RequireContent = requireContent;
    }
}
