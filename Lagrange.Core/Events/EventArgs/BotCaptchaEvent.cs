namespace Lagrange.Core.Events.EventArgs;

public class BotCaptchaEvent(string captchaUrl) : EventBase
{
    public string CaptchaUrl { get; } = captchaUrl;
    
    public override string ToEventMessage() => $"{nameof(BotCaptchaEvent)}: Captcha required, URL: {CaptchaUrl}";
}