namespace Lagrange.Core.Internal.Packets.Login;

using Lagrange.Proto;

[ProtoPackable]
public partial class QrLogin
{
    [ProtoMember(1)] public uint ScanResult { get; set; }
    
    [ProtoMember(2)] public SafeInfo SafeInfo { get; set; } = new();
    
    [ProtoMember(3)] public RiskInfo RiskInfo { get; set; } = new();
    
    [ProtoMember(4)] public RejectInfo? RejectInfo { get; set; } = new();
    
    [ProtoMember(5)] public TipsCtrl TipsCtrl { get; set; } = new();
    
    [ProtoMember(6)] public AutoRenewTicketInfo AutoRenewTicketInfo { get; set; } = new();
}

[ProtoPackable]
public partial class SafeInfo
{
    [ProtoMember(1)] public string TipsTemplate { get; set; } = string.Empty;
    
    [ProtoMember(2)] public string LoginLoc { get; set; } = string.Empty;
    
    [ProtoMember(3)] public string AppName { get; set; } = string.Empty;
    
    [ProtoMember(4)] public string NewTipsTemplate { get; set; } = string.Empty;
}

[ProtoPackable]
public partial class RiskInfo
{
    [ProtoMember(1)] public string TipsTemplate { get; set; } = string.Empty;
    
    [ProtoMember(2)] public string LoginLoc { get; set; } = string.Empty;
    
    [ProtoMember(3)] public string AppName { get; set; } = string.Empty;
    
    [ProtoMember(4)] public string NewTipsTemplate { get; set; } = string.Empty;
}

[ProtoPackable]
public partial class RejectInfo
{
    [ProtoMember(1)] public string Tips { get; set; } = string.Empty;
}

[ProtoPackable]
public partial class AutoRenewTicketInfo
{
    [ProtoMember(1)] public bool IsShowSwitch { get; set; }
    
    [ProtoMember(2)] public bool IsOpenSwitch { get; set; }
    
    [ProtoMember(3)] public string TipsTitle { get; set; } = string.Empty;
    
    [ProtoMember(4)] public string TipsTemplate { get; set; } = string.Empty;
}

[ProtoPackable]
public partial class TipsCtrl
{
    [ProtoMember(1)] public uint NormalTipsColor { get; set; }
    
    [ProtoMember(2)] public bool SecCheck { get; set; }
    
    [ProtoMember(3)] public string SecCheckTips { get; set; } = string.Empty;
    
    [ProtoMember(4)] public uint SecTipsColor { get; set; }
}