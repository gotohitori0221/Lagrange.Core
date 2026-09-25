namespace Lagrange.Core.Events.EventArgs;

public class BotNewDeviceVerifyEvent(string url) : EventBase
{
    public string Url { get; } = url;
    
    public override string ToEventMessage() => $"[{nameof(BotNewDeviceVerifyEvent)}] URL: {Url}";
}