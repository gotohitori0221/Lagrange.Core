using Lagrange.Core.NativeAPI.NativeModel.Event;

namespace Lagrange.Core.NativeAPI.ReverseEvent.Abstract
{
    public abstract class ReverseEventBase
    {
        public List<IEventStruct> Events = [];
        public virtual void RegisterEventHandler(BotContext context) { }
    }
}