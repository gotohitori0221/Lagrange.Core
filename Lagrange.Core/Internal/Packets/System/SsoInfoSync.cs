using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.System;

[ProtoPackable]
public partial class SsoC2CMsgCookie
{
    [ProtoMember(1)] public ulong C2CLastMsgTime { get; set; }
}

[ProtoPackable]
public partial class SsoC2CSyncInfo
{
    [ProtoMember(1)] public SsoC2CMsgCookie C2CMsgCookie { get; set; } = new();

    [ProtoMember(2)] public ulong C2CLastMsgTime { get; set; }

    [ProtoMember(3)] public SsoC2CMsgCookie LastC2CMsgCookie { get; set; } = new();
}

[ProtoPackable]
public partial class DeviceInfo
{
    [ProtoMember(1)] public string DevName { get; set; } = "";

    [ProtoMember(2)] public string DevType { get; set; } = "";

    [ProtoMember(3)] public string OsVer { get; set; } = "";

    [ProtoMember(4)] public string Brand { get; set; } = "";

    [ProtoMember(5)] public string VendorOsName { get; set; } = "";
}

[ProtoPackable]
public partial class OnlineBusinessInfo
{
    [ProtoMember(1)] public uint NotifySwitch { get; set; }

    [ProtoMember(2)] public uint BindUinNotifySwitch { get; set; }
}

[ProtoPackable]
public partial class RegisterInfo
{
    [ProtoMember(1)] public string Guid { get; set; } = "";

    [ProtoMember(2)] public uint KickPC { get; set; }

    [ProtoMember(3)] public string BuildVer { get; set; } = "";

    [ProtoMember(4)] public uint IsFirstRegisterProxyOnline { get; set; }

    [ProtoMember(5)] public uint LocaleId { get; set; }

    [ProtoMember(6)] public DeviceInfo DeviceInfo { get; set; } = new();

    [ProtoMember(7)] public uint SetMute { get; set; }

    [ProtoMember(8)] public uint RegisterVendorType { get; set; }

    [ProtoMember(9)] public uint RegType { get; set; }

    [ProtoMember(10)] public OnlineBusinessInfo BusinessInfo { get; set; } = new();

    [ProtoMember(11)] public uint BatteryStatus { get; set; }
    
    [ProtoMember(12)] public int? Field12 { get; set; }
}

[ProtoPackable]
public partial class NormalConfig
{
    [ProtoMember(1)] public Dictionary<uint, int> IntCfg { get; set; } = new();
}

[ProtoPackable]
public partial class CurAppState
{
    [ProtoMember(1)] public uint IsDelayRequest { get; set; }

    [ProtoMember(2)] public uint AppStatus { get; set; }

    [ProtoMember(3)] public uint SilenceStatus { get; set; }
}

[ProtoPackable]
public partial class SsoInfoSyncRequest
{
    [ProtoMember(1)] public uint SyncFlag { get; set; }

    [ProtoMember(2)] public uint ReqRandom { get; set; }

    [ProtoMember(4)] public uint CurActiveStatus { get; set; }

    [ProtoMember(5)] public ulong GroupLastMsgTime { get; set; }

    [ProtoMember(6)] public SsoC2CSyncInfo C2CSyncInfo { get; set; } = new();

    [ProtoMember(8)] public NormalConfig NormalConfig { get; set; } = new();

    [ProtoMember(9)] public RegisterInfo RegisterInfo { get; set; } = new();

    [ProtoMember(10)] public Dictionary<uint, uint> Unknown { get; set; } = new();

    [ProtoMember(11)] public CurAppState AppState { get; set; } = new();
}

[ProtoPackable]
public partial class RegisterResponse
{
    [ProtoMember(2)] public string Msg { get; set; } = "";
}

[ProtoPackable]
public partial class SsoSyncInfoResponse
{
    [ProtoMember(7)] public RegisterResponse? RegisterResponse { get; set; }
}