namespace Lagrange.Core.Events;

public abstract class EventBase : System.EventArgs
{
    
    
    
    public long EventTime { get; }

    internal EventBase() => EventTime = DateTimeOffset.Now.ToUnixTimeSeconds();

    public abstract string ToEventMessage();

    public override string ToString() => $"[{EventTime:yyyy-MM-dd HH:mm:ss}] {ToEventMessage()}";
}