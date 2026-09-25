namespace Lagrange.Core.Utility.Binary;

[Flags]
internal enum Prefix : byte
{
    None = 0b0000,
    Int8 = 0b0001,
    Int16 = 0b0010,
    Int32 = 0b0100,
    LengthOnly = 0b0000,
    WithPrefix = 0b1000,
}