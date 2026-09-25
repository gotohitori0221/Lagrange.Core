namespace Lagrange.Core.Events.EventArgs;

public class BotFriendRecallEvent(long peerUin, long authorUin, ulong sequence, string tip) : EventBase
{
    public long PeerUin { get; } = peerUin;

    public long AuthorUin { get; } = authorUin;

    public ulong Sequence { get; } = sequence;

    public string Tip { get; } = tip;

    public override string ToEventMessage()
    {
        return $"{nameof(BotGroupRecallEvent)}: ${AuthorUin} recalled {Sequence} in {PeerUin}";
    }
}
