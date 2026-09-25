namespace Lagrange.Proto.Serialization;

public enum WireType : byte
{
    VarInt = 0,
    Fixed64 = 1,
    LengthDelimited = 2,
    Fixed32 = 5,
    Unknown = 255
}