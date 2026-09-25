using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Notify;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.Event0x2DC, 20, true)]
internal class GroupNudgeProcessor : MsgPushProcessorBase
{
    internal override ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType, PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        if (content is not { Length: >= 7 }) return ValueTask.FromResult(false);

        var packet = new BinaryPacket(content.Value.Span);
        long groupUin = packet.Read<int>();
        _ = packet.Read<byte>();
        var proto = packet.ReadBytes(Prefix.Int16 | Prefix.LengthOnly);
        var greyTip = ProtoHelper.Deserialize<NotifyMessageBody>(proto);

        if (greyTip.SubType != 19 || greyTip.GeneralGrayTip.BusiType != 12) return ValueTask.FromResult(false);

        var parameters = greyTip.GeneralGrayTip.MsgTemplParam.ToDictionary(x => x.Name, x => x.Value);
        if (!parameters.TryGetValue("uin_str1", out var senderText) ||
            !long.TryParse(senderText, out var senderUin) ||
            !parameters.TryGetValue("uin_str2", out var receiverText) ||
            !long.TryParse(receiverText, out var receiverUin))
        {
            return ValueTask.FromResult(false);
        }

        string action = parameters.GetValueOrDefault("action_str")
            ?? parameters.GetValueOrDefault("alt_str1")
            ?? string.Empty;
        string actionImageUrl = parameters.GetValueOrDefault("action_img_url") ?? string.Empty;
        string suffix = parameters.GetValueOrDefault("suffix_str") ?? string.Empty;

        context.EventInvoker.PostEvent(new BotGroupNudgeEvent(
            groupUin,
            senderUin,
            action,
            actionImageUrl,
            receiverUin,
            suffix
        ));
        return ValueTask.FromResult(true);
    }
}