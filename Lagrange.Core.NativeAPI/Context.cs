using Lagrange.Core.NativeAPI.ReverseEvent;

namespace Lagrange.Core.NativeAPI
{
    public class Context
    {
        public Context(BotContext botContext)
        {
            BotContext = botContext;
            EventInvoker = new ReverseEventInvoker(BotContext);
            SendMessageContext = new SendMessageContext(BotContext);
        }
        public BotContext BotContext { get; set; }
        public ReverseEventInvoker EventInvoker { get; set; }
        public SendMessageContext SendMessageContext { get; set; }
    }
}