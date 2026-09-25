using Lagrange.Core.Common;
using Lagrange.Core.Events;

namespace Lagrange.Core.Services;




[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class EventSubscribeAttribute<TEvent>(Protocols protocols) : EventSubscribeAttribute(typeof(TEvent), protocols)
    where TEvent : ProtocolEvent;




[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class EventSubscribeAttribute(Type eventType, Protocols protocols) : Attribute
{
    public Type EventType { get; } = eventType;

    public Protocols Protocols { get; } = protocols;
}
