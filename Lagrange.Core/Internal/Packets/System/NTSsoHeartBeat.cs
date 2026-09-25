using Lagrange.Proto;

namespace Lagrange.Core.Internal.Packets.System;

[ProtoPackable]
internal partial class SsoHeartBeatRequest
{
    [ProtoMember(1)] public uint Type { get; set; }

    [ProtoMember(2)] public SilenceState LocalSilence { get; set; } = new();

    [ProtoMember(3)] public uint BatteryState { get; set; }

    [ProtoMember(4)] public ulong Time { get; set; }
    
    public void SetBatteryState(byte batteryLevel, bool isCharging)
    {
        BatteryState = ((uint)batteryLevel & 0x7F) | ((uint)(isCharging ? 1 : 0) << 7);
    }
}

[ProtoPackable]
internal partial class SsoHeartBeatResponse
{
    [ProtoMember(3)] public ulong Interval { get; set; }
}

[ProtoPackable]
internal partial class SilenceState
{
    [ProtoMember(1)] public uint LocalSilence { get; set; }
}