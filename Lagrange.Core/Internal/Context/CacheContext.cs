using System.Collections.Concurrent;
using Lagrange.Core.Common.Entity;
using Lagrange.Core.Internal.Events.System;

namespace Lagrange.Core.Internal.Context;

internal class CacheContext(BotContext context)
{
    private List<BotFriend>? _friends;
    private List<BotGroup>? _groups;
    private readonly ConcurrentDictionary<long, string> _uinToUid = new();
    private readonly ConcurrentDictionary<string, long> _uidToUin = new();
    private readonly ConcurrentDictionary<long, List<BotGroupMember>> _members = new();
    private readonly ConcurrentDictionary<long, Dictionary<long, BotGroupMember>> _memberIndex = new();
    private readonly Dictionary<int, BotFriendCategory> _categories = [];
    private readonly Dictionary<string, BotStranger> _strangersWithUid = [];
    private readonly Dictionary<long, BotStranger> _strangersWithUin = [];
    private readonly SemaphoreSlim _strangersLock = new(1);

    public async Task<List<BotFriend>> GetFriendList(bool refresh = false)
    {
        if (refresh || _friends == null) Interlocked.Exchange(ref _friends, await FetchFriends());
        return _friends;
    }

    public async Task<List<BotGroup>> GetGroupList(bool refresh = false)
    {
        if (refresh || _groups == null) Interlocked.Exchange(ref _groups, await FetchGroups());
        return _groups;
    }

    public async Task<List<BotGroupMember>> GetMemberList(long groupUin, bool refresh = false)
    {
        if (refresh || !_members.TryGetValue(groupUin, out var members))
        {
            members = _members[groupUin] = await FetchGroupMembers(groupUin);
            IndexMembers(groupUin, members);
        }

        return members;
    }

    private void IndexMembers(long groupUin, List<BotGroupMember> members)
    {
        var index = new Dictionary<long, BotGroupMember>(members.Count);
        foreach (var member in members) index[member.Uin] = member;
        _memberIndex[groupUin] = index;
    }

    public async Task<List<BotFriendCategory>> GetCategories(bool refresh = false)
    {
        if (refresh || _friends == null || _categories.Count == 0) Interlocked.Exchange(ref _friends, await FetchFriends());
        return _categories.Values.ToList();
    }

    public async Task<BotFriend?> ResolveFriend(long uin)
    {
        if (_friends == null) Interlocked.Exchange(ref _friends, await FetchFriends());
        var friend = _friends?.FirstOrDefault(f => f.Uin == uin);
        if (friend == null)
        {
            Interlocked.Exchange(ref _friends, await FetchFriends());
            friend = _friends?.FirstOrDefault(f => f.Uin == uin);
        }

        return friend;
    }

    public async Task<(BotGroup, BotGroupMember)?> ResolveMember(long groupUin, long memberUin)
    {
        var group = await ResolveGroup(groupUin);
        if (group == null) return null;
        if (!_memberIndex.TryGetValue(groupUin, out var index))
        {
            if (!_members.TryGetValue(groupUin, out var members)) members = _members[groupUin] = await FetchGroupMembers(groupUin);
            IndexMembers(groupUin, members);
            index = _memberIndex[groupUin];
        }

        return index.TryGetValue(memberUin, out var member) ? (group, member) : null;
    }

    public async Task<BotGroup?> ResolveGroup(long groupUin)
    {
        if (_groups == null) Interlocked.Exchange(ref _groups, await FetchGroups());
        var group = _groups?.FirstOrDefault(f => f.GroupUin == groupUin);
        if (group == null)
        {
            Interlocked.Exchange(ref _groups, await FetchGroups());
            group = _groups?.FirstOrDefault(f => f.GroupUin == groupUin);
        }

        return group;
    }

    public async Task<BotStranger> ResolveStranger(long uin)
    {
        if (_strangersWithUin.TryGetValue(uin, out BotStranger? stranger)) return stranger;
        await _strangersLock.WaitAsync();
        try
        {
            if (_strangersWithUin.TryGetValue(uin, out stranger)) return stranger;
            stranger = await FetchStranger(uin);
            CacheStranger(stranger);
            return stranger;
        }
        finally { _strangersLock.Release(); }
    }

    public async Task<BotStranger> ResolveStranger(string uid)
    {
        if (_strangersWithUid.TryGetValue(uid, out BotStranger? stranger)) return stranger;
        await _strangersLock.WaitAsync();
        try
        {
            if (_strangersWithUid.TryGetValue(uid, out stranger)) return stranger;
            stranger = await FetchStranger(uid);
            CacheStranger(stranger);
            return stranger;
        }
        finally { _strangersLock.Release(); }
    }

    private void CacheStranger(BotStranger stranger)
    {
        _strangersWithUin[ stranger.Uin] = stranger;
        _strangersWithUid[stranger.Uid] = stranger;
        _uidToUin[stranger.Uid] = stranger.Uin;
        _uinToUid[stranger.Uin] = stranger.Uid;
    }

    public string? ResolveCachedUid(long uin) => _uinToUid.GetValueOrDefault(uin);

    public async Task<string?> ResolveGroupMemberUidAsync(long groupUin, long targetUin)
    {
        if (_uinToUid.TryGetValue(targetUin, out string? uid)) return uid;
        if (!_members.ContainsKey(groupUin)) await GetMemberList(groupUin);
        if (_uinToUid.TryGetValue(targetUin, out uid)) return uid;
        await GetMemberList(groupUin, true);
        return _uinToUid.GetValueOrDefault(targetUin);
    }

    public long ResolveUin(string uid)
    {
        if (_uidToUin.TryGetValue(uid, out long value)) return value;
        _ = PrefetchStrangerAsync(uid);
        return 0;
    }

    public async Task<long> ResolveUinAsync(string? uid)
    {
        if (string.IsNullOrEmpty(uid)) return 0;
        if (_uidToUin.TryGetValue(uid, out long value)) return value;
        try { return (await ResolveStranger(uid)).Uin; }
        catch { return 0; }
    }

    public async Task<long> ResolveGroupMemberUinAsync(long groupUin, string? uid)
    {
        if (string.IsNullOrEmpty(uid)) return 0;
        if (_uidToUin.TryGetValue(uid, out long uin)) return uin;

        if (!_members.TryGetValue(groupUin, out var members)) members = await GetMemberList(groupUin);
        var member = members.FirstOrDefault(item => item.Uid == uid);
        if (member != null) return member.Uin;

        return await ResolveUinAsync(uid);
    }

    public void UpdateMemberShutUp(long groupUin, long memberUin, long shutUpTimestamp)
    {
        if (!_members.TryGetValue(groupUin, out var members)) return;
        int index = members.FindIndex(member => member.Uin == memberUin);
        if (index < 0) return;

        var old = members[index];
        var updated = new List<BotGroupMember>(members);
        updated[index] = new BotGroupMember(
            old.Group, old.Uin, old.Uid, old.Nickname, old.Permission, old.GroupLevel,
            old.MemberCard, old.SpecialTitle, old.JoinTime, old.LastMsgTime, shutUpTimestamp);

        _members[groupUin] = updated;
        IndexMembers(groupUin, updated);
    }

    private async Task PrefetchStrangerAsync(string uid)
    {
        try { CacheStranger(await ResolveStranger(uid)); }
        catch { }
    }

    private async Task<List<BotFriend>> FetchFriends()
    {
        var friends = new List<BotFriend>();
        byte[]? cookie = null;
        do
        {
            var result = await context.EventContext.SendEvent<FetchFriendsEventResp>(new FetchFriendsEventReq(cookie));
            cookie = result.Cookie;
            friends.AddRange(result.Friends);
            foreach (var category in result.Category) _categories[category.Id] = category;
            foreach (var friend in result.Friends)
            {
                _uinToUid[friend.Uin] = friend.Uid;
                _uidToUin[friend.Uid] = friend.Uin;
            }
        } while (cookie != null);
        return friends;
    }

    private async Task<List<BotGroup>> FetchGroups()
    {
        var result = await context.EventContext.SendEvent<FetchGroupsEventResp>(new FetchGroupsEventReq());
        return result.Groups;
    }

    private async Task<List<BotGroupMember>> FetchGroupMembers(long groupUin)
    {
        var members = new List<BotGroupMember>();
        byte[]? cookie = null;
        do
        {
            var result = await context.EventContext.SendEvent<FetchGroupMembersEventResp>(new FetchGroupMembersEventReq(groupUin, cookie));
            cookie = result.Cookie;
            members.AddRange(result.GroupMembers);
            foreach (var member in result.GroupMembers)
            {
                _uinToUid[member.Uin] = member.Uid;
                _uidToUin[member.Uid] = member.Uin;
            }
        } while (cookie != null);
        return members;
    }

    private async Task<BotStranger> FetchStranger(long uin)
    {
        var result = await context.EventContext.SendEvent<FetchStrangerEventResp>(new FetchStrangerByUinEventReq(uin));
        return result.Stranger;
    }

    private async Task<BotStranger> FetchStranger(string uid)
    {
        var result = await context.EventContext.SendEvent<FetchStrangerEventResp>(new FetchStrangerByUidEventReq(uid));
        return result.Stranger;
    }
}