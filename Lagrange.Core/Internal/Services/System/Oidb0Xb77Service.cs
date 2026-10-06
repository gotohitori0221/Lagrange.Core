using Lagrange.Core.Common;
using Lagrange.Core.Internal.Events.System;
using Lagrange.Core.Internal.Packets.Service;
using Lagrange.Core.Services;
using Lagrange.Core.Utility;

namespace Lagrange.Core.Internal.Services.System;

[EventSubscribe<Oidb0Xb77EventReq>(Protocols.All)]
[Service("OidbSvc.0xb77_9")]
internal class Oidb0Xb77Service : BaseService<Oidb0Xb77EventReq, Oidb0Xb77EventResp>
{
    protected override ValueTask<ReadOnlyMemory<byte>> Build(Oidb0Xb77EventReq input, BotContext context)
    {
        var oidb = new Oidb
        {
            Command = 0xb77,
            Service = 9,
            Body = input.Body,
        };

        return ValueTask.FromResult(ProtoHelper.Serialize(oidb));
    }

    protected override ValueTask<Oidb0Xb77EventResp> Parse(ReadOnlyMemory<byte> input, BotContext context)
    {
        var oidb = ProtoHelper.Deserialize<Oidb>(input.Span);
        return ValueTask.FromResult(new Oidb0Xb77EventResp(oidb.Result, oidb.Message, oidb.Body));
    }
}
