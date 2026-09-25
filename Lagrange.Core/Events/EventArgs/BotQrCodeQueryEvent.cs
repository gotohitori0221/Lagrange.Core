namespace Lagrange.Core.Events.EventArgs;

public class BotQrCodeQueryEvent(BotQrCodeQueryEvent.TransEmpState state, long uin = 0) : EventBase
{
    public TransEmpState State { get; } = state;

    
    public long Uin { get; } = uin;

    public override string ToEventMessage() => $"[{nameof(BotQrCodeQueryEvent)}] State: {State} Uin: {Uin}";
    
    public enum TransEmpState : byte
    {
        Confirmed = 0,
        CodeExpired = 17,
        WaitingForScan = 48,
        WaitingForConfirm = 53,
        Canceled = 54,
        Invalid = 144
    }
}