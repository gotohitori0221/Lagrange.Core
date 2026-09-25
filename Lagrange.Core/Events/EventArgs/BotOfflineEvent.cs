namespace Lagrange.Core.Events.EventArgs;

public class BotOfflineEvent(BotOfflineEvent.Reasons reason, (string, string)? tips) : EventBase
{
    public Reasons Reason { get; } = reason;

    public (string Tag, string Message)? Tips { get; } = tips;

    public override string ToEventMessage() => $"{nameof(BotOfflineEvent)}: {Reason} ({Tips?.Tag}, {Tips?.Message})";

    public enum Reasons
    {
        Logout,
        Kicked,
        Disconnected,
    }
}