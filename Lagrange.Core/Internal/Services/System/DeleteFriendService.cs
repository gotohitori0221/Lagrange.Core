using Lagrange.Core.Common;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Services;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Services.System;

[EventSubscribe<DeleteFriendEventReq>(Protocols.All)]
[Service("OidbSvcTrpcTcp.0x126b_0")]
internal class DeleteFriendService : BaseService<DeleteFriendEventReq, DeleteFriendEventResp>
{
    protected override ValueTask<ReadOnlyMemory<byte>> Build(DeleteFriendEventReq input, BotContext context)
    {
        var request = new D126BReqBody
        {
            Field1 = new D126BReqBodyField1
            {
                TargetUid = input.TargetUid,
                Block = input.Block,
                Field4 = true,
            }
        };

        return ValueTask.FromResult(ProtoHelper.Serialize(new Oidb
        {
            Command = 0x126B,
            Service = 0,
            Body = ProtoHelper.Serialize(request)
        }));
    }

    protected override ValueTask<DeleteFriendEventResp> Parse(ReadOnlyMemory<byte> input, BotContext context)
    {
        var oidb = ProtoHelper.Deserialize<Oidb>(input.Span);
        return ValueTask.FromResult(new DeleteFriendEventResp((int)oidb.Result));
    }
}
