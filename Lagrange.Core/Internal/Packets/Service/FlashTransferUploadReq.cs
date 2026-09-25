using Lagrange.Proto;

#pragma warning disable CS8618 
namespace Lagrange.Core.Internal.Packets.Service;

[ProtoPackable]
internal partial class FlashTransferUploadReq
{
    [ProtoMember(1)] public uint FieId1 { get; set; } 
    [ProtoMember(2)] public uint AppId { get; set; } 
    [ProtoMember(3)] public uint FileId3 { get; set; } 
    [ProtoMember(107)] public FlashTransferUploadBody Body { get; set; }
}

[ProtoPackable]
internal partial class FlashTransferUploadBody
{
    [ProtoMember(1)] public byte[] FieId1 { get; set; } 
    [ProtoMember(2)] public string UKey { get; set; }
    [ProtoMember(3)] public uint Start { get; set; } 
    [ProtoMember(4)] public uint End { get; set; } 
    [ProtoMember(5)] public byte[] Sha1 { get; set; }
    [ProtoMember(6)] public FlashTransferSha1StateV Sha1StateV { get; set; }
    [ProtoMember(7)] public byte[] Body { get; set; }
}

[ProtoPackable]
internal partial class FlashTransferSha1StateV
{
    [ProtoMember(1)] public List<byte[]> State { get; set; }
}