using Lagrange.Proto;
#pragma warning disable CS8618 

namespace Lagrange.Core.Internal.Packets.Service;

[ProtoPackable]
internal partial class FlashTransferUploadResp
{
    [ProtoMember(5)] public string Status { get; set; }
}