using Lagrange.Core.Common.Entity;
using Lagrange.Core.Internal.Logic;

namespace Lagrange.Core.Common.Interface;

public static class BotExt
{
    public static Task<bool> Login(this BotContext context, long uin, string password, CancellationToken token = default) =>
        context.EventContext.GetLogic<WtExchangeLogic>().Login(uin, password, token);
    
    public static Task<bool> Login(this BotContext context, CancellationToken token = default) =>
        context.EventContext.GetLogic<WtExchangeLogic>().Login(0, null, token);

    public static Task<bool> Logout(this BotContext context) =>
        context.EventContext.GetLogic<WtExchangeLogic>().Logout();
    
    public static Task<long> ResolveUinByQid(this BotContext context, string qid) =>
        context.EventContext.GetLogic<WtExchangeLogic>().ResolveUinByQid(qid);

    public static bool SubmitCaptcha(this BotContext context, string ticket, string randStr) =>
        context.EventContext.GetLogic<WtExchangeLogic>().SubmitCaptcha(ticket, randStr);

    public static bool SubmitSMSCode(this BotContext context, string code) =>
        context.EventContext.GetLogic<WtExchangeLogic>().SubmitSMSCode(code);

    public static ValueTask<BotSsoPacket> SendPacket(this BotContext context, BotSsoPacket packet, RequestType requestType = RequestType.D2Auth, EncryptType encryptType = EncryptType.EncryptD2Key)
        => context.EventContext.GetLogic<OperationLogic>().SendPacket(packet, requestType, encryptType);
}