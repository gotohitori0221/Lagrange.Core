using Lagrange.Core.Common;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Services;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Services.System;

[EventSubscribe<GroupSetAdminEventReq>(Protocols.All)]
[Service("OidbSvcTrpcTcp.0x1096_1")]
internal class GroupSetAdminService : BaseService<GroupSetAdminEventReq, GroupSetAdminEventResp>
{
    protected override ValueTask<ReadOnlyMemory<byte>> Build(GroupSetAdminEventReq input, BotContext context)
    {
        var request = new D1096ReqBody
        {
            GroupUin = input.GroupUin,
            Uid = input.Uid,
            IsAdmin = input.IsAdmin,
        };

        return ValueTask.FromResult(ProtoHelper.Serialize(new Oidb
        {
            Command = 0x1096,
            Service = 1,
            Body = ProtoHelper.Serialize(request)
        }));
    }

    protected override ValueTask<GroupSetAdminEventResp> Parse(ReadOnlyMemory<byte> input, BotContext context)
    {
        var oidb = ProtoHelper.Deserialize<Oidb>(input.Span);
        return ValueTask.FromResult(new GroupSetAdminEventResp((int)oidb.Result));
    }
}
