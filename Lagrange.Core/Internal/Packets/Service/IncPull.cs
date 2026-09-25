using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618

[ProtoPackable]
internal partial class IncPullRequest
{
    [ProtoMember(2)] public uint ReqCount { get; set; } 
    
    [ProtoMember(3)] public long Time { get; set; } 
    
    [ProtoMember(4)] public uint LocalSeq { get; set; } 
    
    [ProtoMember(5)] public byte[]? Cookie { get; set; } 
    
    [ProtoMember(6)] public int Flag { get; set; } 
    
    [ProtoMember(7)] public uint ProxySeq { get; set; } 
    
    [ProtoMember(10001)] public List<IncPullRequestBiz> RequestBiz { get; set; } 
    
    [ProtoMember(10002)] public List<uint> ExtSnsFlagKey { get; set; } = []; 

    [ProtoMember(10003)] public List<uint> ExtPrivateIdListKey { get; set; } = []; 
}

[ProtoPackable]
internal partial class IncPullRequestBiz
{
    [ProtoMember(1)] public int BizType { get; set; }
    
    [ProtoMember(2)] public IncPullRequestBizBusi BizData { get; set; } 
}

[ProtoPackable]
internal partial class IncPullRequestBizBusi
{
    [ProtoMember(1)] public List<int> ExtBusi { get; set; } 
}

[ProtoPackable]
internal partial class IncPullResponse
{
    [ProtoMember(1)] public uint Seq { get; set; } 
    
    [ProtoMember(2)] public byte[]? Cookie { get; set; } 
    
    [ProtoMember(3)] public bool IsEnd { get; set; } 
    
    [ProtoMember(6)] public long Time { get; set; } 
    
    [ProtoMember(7)] public long SelfUin { get; set; }
    
    [ProtoMember(8)] public uint SmallSeq { get; set; } 
    
    [ProtoMember(101)] public List<IncPullResponseFriend> FriendList { get; set; }
    
    [ProtoMember(102)] public List<IncPullResponseCategory> Category { get; set; }
}

[ProtoPackable]
internal partial class IncPullResponseFriend
{
    [ProtoMember(1)] public string Uid { get; set; }
    
    [ProtoMember(2)] public int CategoryId { get; set; } 
    
    [ProtoMember(3)] public long Uin { get; set; }
    
    [ProtoMember(10001)] public Dictionary<int, IncPullResponseSubBiz> SubBiz { get; set; } 
}

[ProtoPackable]
internal partial class IncPullResponseSubBiz
{
    [ProtoMember(1)] public Dictionary<int, int> NumData { get; set; }
    
    [ProtoMember(2)] public Dictionary<int, string> Data { get; set; }
}

[ProtoPackable]
internal partial class IncPullResponseCategory
{
    [ProtoMember(1)] public int CategoryId { get; set; } 
    
    [ProtoMember(2)] public string CategoryName { get; set; } 
    
    [ProtoMember(3)] public int CategoryMemberCount { get; set; } 
    
    [ProtoMember(4)] public int CatogorySortId { get; set; } 
}