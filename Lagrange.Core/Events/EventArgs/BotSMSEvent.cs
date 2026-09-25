namespace Lagrange.Core.Events.EventArgs;

public class BotSMSEvent(string? url, string phone) : EventBase
{
    public string? Url { get; } = url;

    public string Phone { get; } = phone;

    public override string ToEventMessage() => $"{nameof(BotSMSEvent)} {Phone} | URL: {Url}";
}