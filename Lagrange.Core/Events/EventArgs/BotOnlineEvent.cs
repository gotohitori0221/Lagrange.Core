namespace Lagrange.Core.Events.EventArgs;

public class BotOnlineEvent(BotOnlineEvent.Reasons reason) : EventBase
{
    public enum Reasons
    {
        Login,
        Reconnect,
    }

    public Reasons Reason { get; } = reason;

    public override string ToEventMessage() => $"{nameof(BotOnlineEvent)}: {Reason}";
}