using Lagrange.Core.Common;
using Lagrange.Core.Internal.Events.Message;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Services;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Services.Message;

[EventSubscribe<MarkReadedEventReq>(Protocols.All)]
[Service("trpc.msg.msg_svc.MsgService.SsoReadedReport")]
internal class MarkReadedService : BaseService<MarkReadedEventReq, MarkReadedEventResp>
{
    protected override ValueTask<ReadOnlyMemory<byte>> Build(MarkReadedEventReq input, BotContext context)
    {
        var packet = input.TargetUid == null
            ? new SsoReadedReport
            {
                Group = new SsoReadedReportGroup
                {
                    GroupUin = input.GroupUin ?? 0,
                    StartSequence = input.StartSequence
                }
            }
            : new SsoReadedReport
            {
                C2C = new SsoReadedReportC2C
                {
                    TargetUid = input.TargetUid,
                    Time = input.Time,
                    StartSequence = input.StartSequence
                }
            };

        return ValueTask.FromResult(ProtoHelper.Serialize(packet));
    }

    protected override ValueTask<MarkReadedEventResp> Parse(ReadOnlyMemory<byte> input, BotContext context)
    {
        return ValueTask.FromResult(new MarkReadedEventResp(0));
    }
}
