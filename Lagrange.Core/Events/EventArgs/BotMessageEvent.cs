using Lagrange.Core.Message;

namespace Lagrange.Core.Events.EventArgs;

public class BotMessageEvent(BotMessage message, ReadOnlyMemory<byte> rawMessage) : EventBase
{
    public BotMessage Message { get; } = message;
    
    internal ReadOnlyMemory<byte> RawMessage { get; } = rawMessage;
    
    public override string ToEventMessage() => $"{nameof(EventBase)} {Message}";
}