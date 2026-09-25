using System.Linq;
using Lagrange.Core.Common;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Services;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Services.System;

[EventSubscribe<FetchPinsEventReq>(Protocols.All)]
[Service("OidbSvcTrpcTcp.0x12b3_0")]
internal class FetchPinsService : BaseService<FetchPinsEventReq, FetchPinsEventResp>
{
    protected override ValueTask<ReadOnlyMemory<byte>> Build(FetchPinsEventReq input, BotContext context)
    {
        return ValueTask.FromResult(ProtoHelper.Serialize(new Oidb
        {
            Command = 0x12B3,
            Service = 0,
            Body = ReadOnlyMemory<byte>.Empty
        }));
    }

    protected override ValueTask<FetchPinsEventResp> Parse(ReadOnlyMemory<byte> input, BotContext context)
    {
        var oidb = ProtoHelper.Deserialize<Oidb>(input.Span);
        if (oidb.Result != 0)
        {
            return ValueTask.FromResult(new FetchPinsEventResp([], []));
        }

        var body = ProtoHelper.Deserialize<D12B3RspBody>(oidb.Body.Span);
        var friendUids = body.Friends?.Select(f => f.Uid).ToList() ?? [];
        var groupUins = body.Groups?.Select(g => g.Uin).ToList() ?? [];

        return ValueTask.FromResult(new FetchPinsEventResp(friendUids, groupUins));
    }
}
