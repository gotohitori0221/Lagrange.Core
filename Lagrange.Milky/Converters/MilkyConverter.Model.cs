using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Common;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Common.Interface;
using Lagrange.Core.Message;
using Lagrange.Milky.Events.Converters;
using Lagrange.Milky.Extensions;
using Lagrange.Milky.Models;

namespace Lagrange.Milky.Converters;

public partial class MilkyConverter
{
    public Friend ToFriend(BotFriend friend) => new()
    {
        UserId = friend.Uin,
        Nickname = friend.Nickname,
        Sex = friend.Gender switch
        {
            BotGender.Male => "male",
            BotGender.Female => "female",
            _ => "unknown"
        },
        Qid = friend.Qid,
        Remark = friend.Remarks,
        Category = ToFriendCategory(friend.Category),
    };

    public Group ToGroup(BotGroup group) => new()
    {
        GroupId = group.Uin,
        GroupName = group.GroupName,
        MemberCount = group.MemberCount,
        MaxMemberCount = group.MaxMember,
        Remark = group.GroupRemark ?? string.Empty,
        CreatedTime = group.CreateTime,
        Description = group.Description ?? string.Empty,
        Question = group.Question ?? string.Empty,
        Announcement = group.Announcement ?? string.Empty,
    };

    public GroupMember ToGroupMember(BotGroupMember member) => new()
    {
        UserId = member.Uin,
        Nickname = member.Nickname,
        Sex = member.Gender switch
        {
            BotGender.Male => "male",
            BotGender.Female => "female",
            _ => "unknown"
        },
        GroupId = member.Group.Uin,
        Card = member.MemberCard ?? string.Empty,
        Title = member.SpecialTitle ?? string.Empty,
        Level = member.GroupLevel,
        Role = member.Permission switch
        {
            GroupMemberPermission.Owner => "owner",
            GroupMemberPermission.Admin => "admin",
            GroupMemberPermission.Member => "member",
            _ => throw new NotSupportedException(),
        },
        JoinTime = member.JoinTime,
        LastSentTime = member.LastMsgTime,
        ShutUpEndTime = member.ShutUpTimestamp == 0 ? null : member.ShutUpTimestamp
    };

    public async Task<GroupMember> ToGroupMemberAsync(BotGroupMember member, CancellationToken ct = default)
    {
        var result = ToGroupMember(member);
        if (result.Sex != "unknown" || member.Uin == 0) return result;

        if (_memberSexCache.TryGetValue(member.Uin, out var cached))
        {
            result.Sex = cached;
            return result;
        }

        result.Sex = await FetchMemberSexAsync(member.Uin, ct);
        return result;
    }

    
    
    
    
    
    
    
    
    public async Task PrefetchMemberSexAsync(IEnumerable<BotMessage> messages, CancellationToken ct = default)
    {
        HashSet<long>? uins = null;
        foreach (var message in messages)
        {
            if (message.Contact is not BotGroupMember member) continue;
            if (member.Uin == 0 || member.Gender != BotGender.Unknown) continue;
            if (_memberSexCache.ContainsKey(member.Uin)) continue;
            (uins ??= []).Add(member.Uin);
        }

        if (uins is null || uins.Count == 0) return;

        using var semaphore = new SemaphoreSlim(8);
        var tasks = uins.Select(async uin =>
        {
            await semaphore.WaitAsync(ct);
            try { await FetchMemberSexAsync(uin, ct); }
            finally { semaphore.Release(); }
        });
        await Task.WhenAll(tasks);
    }

    private async Task<string> FetchMemberSexAsync(long uin, CancellationToken ct)
    {
        if (_memberSexCache.TryGetValue(uin, out var cached)) return cached;

        try
        {
            
            
            var stranger = await _lagrange.FetchStranger(uin).WaitAsync(TimeSpan.FromSeconds(3), ct);
            var sex = stranger.Gender switch
            {
                BotGender.Male => "male",
                BotGender.Female => "female",
                _ => "unknown",
            };
            _memberSexCache[uin] = sex;
            return sex;
        }
        catch
        {
            
            return "unknown";
        }
    }

    private FriendCategory ToFriendCategory(BotFriendCategory category) => new()
    {
        CategoryId = category.Id,
        CategoryName = category.Name,
    };

    public GroupNotificationBase ToGroupNotification(BotGroupNotificationBase notification) => notification switch
    {
        BotGroupJoinNotification join => new JoinRequestGroupNotification
        {
            GroupId = join.GroupUin,
            NotificationSeq = (long)join.Sequence,
            IsFiltered = join.IsFiltered,
            InitiatorId = join.TargetUin,
            State = join.State switch
            {
                BotGroupNotificationState.Wait => "pending",
                BotGroupNotificationState.Accept => "accepted",
                BotGroupNotificationState.Reject => "rejected",
                BotGroupNotificationState.Ignore => "ignored",
                _ => throw new NotSupportedException(),
            },
            OperatorId = join.OperatorUin,
            Comment = join.Comment,
        },
        BotGroupSetAdminNotification groupSet => new AdminChangeGroupNotification
        {
            GroupId = groupSet.GroupUin,
            NotificationSeq = (long)groupSet.Sequence,
            TargetUserId = groupSet.TargetUin,
            IsSet = true,
            OperatorId = groupSet.OperatorUin,
        },
        BotGroupUnsetAdminNotification groupUnset => new AdminChangeGroupNotification
        {
            GroupId = groupUnset.GroupUin,
            NotificationSeq = (long)groupUnset.Sequence,
            TargetUserId = groupUnset.TargetUin,
            IsSet = false,
            OperatorId = groupUnset.OperatorUin,
        },
        BotGroupKickNotification kick => new KickGroupNotification
        {
            GroupId = kick.GroupUin,
            NotificationSeq = (long)kick.Sequence,
            TargetUserId = kick.TargetUin,
            OperatorId = kick.OperatorUin,
        },
        BotGroupExitNotification exit => new QuitGroupNotification
        {
            GroupId = exit.GroupUin,
            NotificationSeq = (long)exit.Sequence,
            TargetUserId = exit.TargetUin,
        },
        BotGroupInviteNotification invite => new InvitedJoinRequestGroupNotification
        {
            GroupId = invite.GroupUin,
            NotificationSeq = (long)invite.Sequence,
            InitiatorId = invite.InviterUin,
            TargetUserId = invite.TargetUin,
            State = invite.State switch
            {
                BotGroupNotificationState.Wait => "pending",
                BotGroupNotificationState.Accept => "accepted",
                BotGroupNotificationState.Reject => "rejected",
                BotGroupNotificationState.Ignore => "ignored",
                _ => throw new NotSupportedException(),
            },
            OperatorId = invite.OperatorUin,
        },
        _ => throw new NotSupportedException(),
    };

    public GroupFile ToGroupFile(BotFileEntry entry, long groupId) => new()
    {
        GroupId = groupId,
        FileId = entry.FileId,
        FileName = entry.FileName,
        ParentFolderId = entry.ParentDirectory,
        FileSize = (long)entry.FileSize,
        UploadedTime = entry.UploadedTime,
        ExpireTime = entry.ExpireTime,
        UploaderId = entry.UploaderUin,
        DownloadedTimes = (int)entry.DownloadedTimes,
    };

    public GroupFolder ToGroupFolder(BotFolderEntry entry, long groupId) => new()
    {
        GroupId = groupId,
        FolderId = entry.FolderId,
        ParentFolderId = entry.ParentFolderId,
        FolderName = entry.FolderName,
        CreatedTime = entry.CreateTime,
        LastModifiedTime = entry.ModifiedTime,
        CreatorId = entry.CreatorUin,
        FileCount = (int)entry.TotalFileCount,
    };

    public FriendRequest ToFriendRequest(BotFriendRequest request) => new()
    {
        Time = request.Time,
        InitiatorId = request.SourceUin,
        InitiatorUid = request.SourceUid,
        TargetUserId = request.TargetUin,
        TargetUserUid = request.TargetUid,
        State = request.EventState switch
        {
            BotFriendRequest.State.Pending => "pending",
            BotFriendRequest.State.Approved => "accepted",
            BotFriendRequest.State.Disapproved => "rejected",
            _ => "pending",
        },
        Comment = request.Comment,
        Via = request.Source,
        IsFiltered = false,
    };
}
