using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Configurations;
using Lagrange.Milky.Converters;
using Lagrange.Milky.Events.Attributes;
using Lagrange.Milky.Models.Messages;

namespace Lagrange.Milky.Events.Converters;

[EventConverter]
public sealed class MessageReceiveConverter(MilkyConverter converter, BotContext lagrange, MilkyConfiguration configuration) : IEventConverter<BotMessageEvent, IncomingMessageBase>
{
    private readonly MilkyConverter _converter = converter;
    private readonly BotContext _lagrange = lagrange;
    private readonly MilkyConfiguration _configuration = configuration;

    public string Name => "message_receive";

    public bool CanConvert(BotMessageEvent @event)
    {
        if (_configuration.Message.IgnoreSelf && @event.Message.Contact.Uin == _lagrange.BotUin) return false;
        return true;
    }

    public async ValueTask<IncomingMessageBase> ConvertAsync(BotMessageEvent @event, CancellationToken ct)
    {
        return await _converter.ToIncomingMessageAsync(@event.Message, ct);
    }
}
