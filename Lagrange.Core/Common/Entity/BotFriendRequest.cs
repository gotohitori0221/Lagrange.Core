namespace Lagrange.Core.Common.Entity;

[Serializable]
public class BotFriendRequest(long targetUin, string targetUid, long sourceUin, string sourceUid, uint eventState, string comment, string source, uint time)
{
    public long TargetUin { get; set; } = targetUin;

    public string TargetUid { get; set; } = targetUid;

    public long SourceUin { get; set; } = sourceUin;

    public string SourceUid { get; set; } = sourceUid;

    public State EventState { get; set; } = (State)eventState;

    public string Comment { get; set; } = comment;

    public string Source { get; set; } = source;

    public long Time { get; set; } = time;

    public enum State
    {
        Pending = 1,
        Disapproved = 2,
        Approved = 3
    }
}
