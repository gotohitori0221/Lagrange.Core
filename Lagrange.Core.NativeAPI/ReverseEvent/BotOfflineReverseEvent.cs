using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel;
using Lagrange.Core.NativeAPI.NativeModel.Event;
using Lagrange.Core.NativeAPI.ReverseEvent.Abstract;

namespace Lagrange.Core.NativeAPI.ReverseEvent
{
    public class BotOfflineReverseEvent : ReverseEventBase
    {
        public override void RegisterEventHandler(BotContext context)
        {
            context.EventInvoker.RegisterEvent<BotOfflineEvent>((ctx, e) => Events.Add((BotOfflineEventStruct)e)
            );
        }
    }
}
