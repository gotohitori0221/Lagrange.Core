using Lagrange.Core.Common;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Services;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Services.System;

[EventSubscribe<SetFriendRequestEventReq>(Protocols.All)]
[Service("OidbSvcTrpcTcp.0xb5d_44")]
internal class SetFriendRequestService : BaseService<SetFriendRequestEventReq, SetFriendRequestEventResp>
{
    protected override ValueTask<ReadOnlyMemory<byte>> Build(SetFriendRequestEventReq input, BotContext context)
    {
        var request = new DB5DReqBody
        {
            Accept = input.Accept ? 3u : 5u,
            TargetUid = input.TargetUid,
        };

        return ValueTask.FromResult(ProtoHelper.Serialize(new Oidb
        {
            Command = 0xB5D,
            Service = 44,
            Body = ProtoHelper.Serialize(request)
        }));
    }

    protected override ValueTask<SetFriendRequestEventResp> Parse(ReadOnlyMemory<byte> input, BotContext context)
    {
        var oidb = ProtoHelper.Deserialize<Oidb>(input.Span);
        return ValueTask.FromResult(new SetFriendRequestEventResp((int)oidb.Result));
    }
}
