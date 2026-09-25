namespace Lagrange.Core.Common.Entity;

public class BotFriend(long uin, string nickname, string uid, string remarks, string personalSign, string qid, BotFriendCategory category) : BotContact
{
    public override long Uin { get; } = uin;

    public override string Nickname { get; } = nickname ?? string.Empty;

    public override string Uid { get; } = uid ?? string.Empty;
    
    public int Age { get; init; }
    
    public BotGender Gender { get; init; }

    public string Remarks { get; } = remarks ?? string.Empty;

    public string PersonalSign { get; } = personalSign ?? string.Empty;

    public string Qid { get; } = qid ?? string.Empty;
    
    public BotFriendCategory Category { get; } = category;
}