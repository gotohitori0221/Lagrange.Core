using Lagrange.Core.Common;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Services;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Services.System;

[EventSubscribe<FriendLikeEventReq>(Protocols.All)]
[Service("OidbSvcTrpcTcp.0x7e5_104")]
internal class FriendLikeService : BaseService<FriendLikeEventReq, FriendLikeEventResp>
{
    protected override ValueTask<ReadOnlyMemory<byte>> Build(FriendLikeEventReq input, BotContext context)
    {
        var request = new D7E5ReqBody
        {
            TargetUid = input.TargetUid,
            Count = input.Count,
        };

        return ValueTask.FromResult(ProtoHelper.Serialize(new Oidb
        {
            Command = 0x7E5,
            Service = 104,
            Body = ProtoHelper.Serialize(request)
        }));
    }

    protected override ValueTask<FriendLikeEventResp> Parse(ReadOnlyMemory<byte> input, BotContext context)
    {
        var oidb = ProtoHelper.Deserialize<Oidb>(input.Span);
        return ValueTask.FromResult(new FriendLikeEventResp((int)oidb.Result));
    }
}
