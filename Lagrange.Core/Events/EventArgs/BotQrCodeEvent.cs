namespace Lagrange.Core.Events.EventArgs;

public class BotQrCodeEvent(string url, byte[] image) : EventBase
{
    public string Url { get; } = url;
    
    public byte[] Image { get; } = image;
    
    public override string ToEventMessage() => $"[{nameof(BotQrCodeEvent)}] URL: {Url}";
}