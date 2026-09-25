using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.NativeAPI.NativeModel;
using Lagrange.Core.NativeAPI.NativeModel.Event;
using Lagrange.Core.NativeAPI.ReverseEvent.Abstract;

namespace Lagrange.Core.NativeAPI.ReverseEvent
{
    public class BotCaptchaReverseEvent : ReverseEventBase
    {
        public override void RegisterEventHandler(BotContext context)
        {
            context.EventInvoker.RegisterEvent<BotCaptchaEvent>((ctx, e) => Events.Add((BotCaptchaEventStruct)e)
            );
        }
    }
}
