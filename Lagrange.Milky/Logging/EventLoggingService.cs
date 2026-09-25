using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core;
using Lagrange.Core.Events;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Milky.Events;
using Lagrange.Milky.Events.Extensions;
using Microsoft.Extensions.Hosting;

namespace Lagrange.Milky.Logging;





public sealed class EventLoggingService : IHostedService, IGenericEventHandler
{
    private readonly BotContext _lagrange;

    public EventLoggingService(BotContext lagrange)
    {
        _lagrange = lagrange;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _lagrange.RegisterConvertibleEvents(this);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _lagrange.UnregisterConvertibleEvents(this);
        return Task.CompletedTask;
    }

    public Task OnEvent<TEvent>(BotContext lagrange, TEvent @event) where TEvent : EventBase
    {
        
        if (@event is BotMessageEvent) return Task.CompletedTask;

        lagrange.LogInfo("Lagrange.Core.BotContext", @event.ToEventMessage());
        return Task.CompletedTask;
    }
}
