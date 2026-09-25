namespace Lagrange.Core.Events.EventArgs;

public class BotGroupEssenceEvent(
    long groupUin,
    ulong messageSequence,
    uint messageTime,
    long senderUin,
    string senderName,
    long operatorUin,
    string operatorName,
    uint operationTime,
    bool isSet) : EventBase
{
    
    public long GroupUin { get; } = groupUin;

    
    public ulong MessageSequence { get; } = messageSequence;

    
    public uint MessageTime { get; } = messageTime;

    
    public long SenderUin { get; } = senderUin;

    
    public string SenderName { get; } = senderName;

    
    public long OperatorUin { get; } = operatorUin;

    
    public string OperatorName { get; } = operatorName;

    
    public uint OperationTime { get; } = operationTime;

    
    public bool IsSet { get; } = isSet;

    public override string ToEventMessage() =>
        $"{nameof(BotGroupEssenceEvent)}: Group({GroupUin}) seq={MessageSequence} {(IsSet ? "set" : "unset")} by {OperatorUin}({OperatorName})";
}
