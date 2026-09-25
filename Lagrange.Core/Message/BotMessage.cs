using System.Collections.Generic;
using System.Text;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Message.Entities;

namespace Lagrange.Core.Message;


public partial class BotMessage
{
    internal BotMessage(BotContact contact, BotContact receiver, long time)
    {
        Contact = contact;
        Receiver = receiver;
        Time = time;
    }

    internal BotMessage(MessageChain chain, BotContact contact, BotContact receiver, long time)
    {
        Entities = chain;
        Contact = contact;
        Receiver = receiver;
        Time = time;
    }

    public BotContact Contact { get; }

    public BotContact Receiver { get; }

    public MessageType Type => Contact switch
    {
        BotGroupMember _ => MessageType.Group,
        BotFriend _ => MessageType.Private,
        BotStranger _ => MessageType.Temp,
        _ => throw new ArgumentOutOfRangeException(nameof(Contact))
    };

    public long Time { get; set; }

    public MessageChain Entities { get; } = [];

    internal ulong MessageId { get; set; }

    internal uint Random { get; init; }

    public ulong Sequence { get; set; }

    public ulong ClientSequence { get; init; } = (ulong)new Random().NextInt64(10000, 99999);

    
    
    
    
    public static BotMessage Restore(
        MessageType type,
        long peerUin,
        long senderUin,
        string senderName,
        ulong sequence,
        ulong clientSequence,
        long time,
        IEnumerable<IMessageEntity> entities)
    {
        BotContact sender;
        BotContact receiver;

        switch (type)
        {
            case MessageType.Group:
                var group = new BotGroup(peerUin, string.Empty, 0, 0, 0, null, null, null);
                sender = new BotGroupMember(group, senderUin, string.Empty, senderName, GroupMemberPermission.Member, 0, null, null, 0, 0, 0);
                receiver = group;
                break;
            case MessageType.Private:
                sender = new BotFriend(senderUin, senderName, string.Empty, string.Empty, string.Empty, string.Empty, new BotFriendCategory(0, string.Empty, 0, 0));
                receiver = sender;
                break;
            default:
                var stranger = new BotStranger(senderUin, senderName, string.Empty, string.Empty, string.Empty, 0, BotGender.Unknown, 0, null, 0, string.Empty, string.Empty, string.Empty, null);
                sender = stranger;
                receiver = stranger;
                break;
        }

        var msg = new BotMessage(sender, receiver, time)
        {
            Sequence = sequence,
        };
        foreach (var e in entities)
            msg.Entities.Add(e);
        return msg;
    }

    public string ToPreviewString()
    {
        var chainBuilder = new StringBuilder();
        chainBuilder.Append($"[{Type}Message]");
        chainBuilder.Append($" [Sender: {Contact.Nickname}({Contact.Uin})");
        if (Contact is BotGroupMember groupMember)
        {
            chainBuilder.Append($" @{groupMember.Group.GroupName}({groupMember.Group.GroupUin})");
            if (!string.IsNullOrEmpty(groupMember.MemberCard))
                chainBuilder.Append($" | Card: {groupMember.MemberCard}");
        }
        chainBuilder.Append(']');
        chainBuilder.Append($" [Id={MessageId}] [Seq={(Type == MessageType.Private ? ClientSequence : Sequence)}] [Time={DateTimeOffset.FromUnixTimeSeconds(Time).ToLocalTime():yyyy-MM-dd HH:mm:ss}]");
        chainBuilder.Append(' ');
        foreach (var entity in Entities)
        {
            chainBuilder.Append(entity.ToPreviewString());
            if (Entities.Last() != entity) chainBuilder.Append(" | ");
        }
        return chainBuilder.ToString();
    }

    public override string ToString() => ToPreviewString();
}