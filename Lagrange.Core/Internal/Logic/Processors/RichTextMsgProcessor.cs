using System.Text.Json;
using System.Web;
using System.Linq;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entities;

namespace Lagrange.Core.Internal.Logic.Processors;

[MsgPushProcessor(MsgType.GroupMessage)]
[MsgPushProcessor(MsgType.PrivateMessage)]
[MsgPushProcessor(MsgType.TempMessage)]
[MsgPushProcessor(MsgType.FriendFileMessage)]
internal class RichTextMsgProcessor : MsgPushProcessorBase
{
    internal override async ValueTask<bool> Handle(BotContext context, MsgType msgType, int subType, PushMessageEvent msgEvt, ReadOnlyMemory<byte>? content)
    {
        var common = msgEvt.MsgPush.CommonMessage;
        var message = await context.EventContext.GetLogic<MessagingLogic>().Parse(common);

        if (message.Entities.Count > 0 && message.Entities[0] is LightAppEntity app && TryHandleLightApp(context, message, app)) return true;

        if (msgType is MsgType.PrivateMessage or MsgType.TempMessage or MsgType.FriendFileMessage &&
            message.Entities.OfType<FileEntity>().FirstOrDefault() is { } file)
        {
            long senderUin = common.RoutingHead.FromUin != 0
                ? common.RoutingHead.FromUin
                : context.CacheContext.ResolveUin(common.RoutingHead.FromUid);

            context.EventInvoker.PostEvent(new BotFriendFileUploadEvent(
                senderUin,
                file.FileName,
                (ulong)file.FileSize,
                file.FileId,
                senderUin == context.BotUin,
                file.FileMd5
            ));
        }

        context.LogInfo("Lagrange.Core.BotContext", message.ToPreviewString());
        context.EventInvoker.PostEvent(new BotMessageEvent(message, msgEvt.Raw));
        return true;
    }

    private bool TryHandleLightApp(BotContext context, BotMessage message, LightAppEntity app)
    {
        if ((app.AppName == "com.tencent.qun.invite" || app.AppName == "com.tencent.qun.invite") && TryHandleQunInvite(context, message, app)) return true;

        return false;
    }

    private bool TryHandleQunInvite(BotContext context, BotMessage message, LightAppEntity app)
    {
        using var document = JsonDocument.Parse(app.Payload);
        var root = document.RootElement;

        string? bizsrc = root.GetProperty("bizsrc").GetString();
        if (app.AppName == "com.tencent.qun.invite" && bizsrc != "qun.invite") return false;

        string? url = root.GetProperty("meta").GetProperty("news").GetProperty("jumpUrl").GetString();
        if (url == null) throw new Exception($"TryHandleQunInvite failed! LightApp: {app.Payload}");

        var query = HttpUtility.ParseQueryString(new Uri(url).Query);
        long uin = uint.Parse(query["groupcode"] ?? throw new Exception($"TryHandleQunInvite failed! LightApp: {app.Payload}"));
        ulong sequence = ulong.Parse(query["msgseq"] ?? throw new Exception($"TryHandleQunInvite failed! LightApp: {app.Payload}"));

        context.EventInvoker.PostEvent(new BotGroupInviteNotificationEvent(new BotGroupInviteNotification(
            uin,
            sequence,
            context.BotUin,
            context.CacheContext.ResolveCachedUid(context.BotUin) ?? string.Empty,
            BotGroupNotificationState.Wait,
            null,
            null,
            message.Contact.Uin,
            message.Contact.Uid,
            false,
            message.Contact is BotGroupMember groupMember ? groupMember.Group.GroupUin : null
        )));

        return true;
    }
}