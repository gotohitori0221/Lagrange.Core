namespace Lagrange.Core.Events.EventArgs;

public class BotGroupRecallEvent(long groupUin, ulong sequence, long authorUin, long operatorUin, string tip) : EventBase
{
    public long GroupUin { get; } = groupUin;

    public ulong Sequence { get; } = sequence;

    public long AuthorUin { get; } = authorUin;

    public long OperatorUin { get; } = operatorUin;

    public string Tip { get; } = tip;

    public override string ToEventMessage()
    {
        return $"{nameof(BotGroupRecallEvent)}: {OperatorUin} recalled {GroupUin}-{Sequence} of {AuthorUin}";
    }
}
