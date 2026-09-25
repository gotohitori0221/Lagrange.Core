namespace Lagrange.Core.Common.Entity;

public class BotSsoPacket
{
    public ReadOnlyMemory<byte> Data { get; }

    public string Command { get; }

    public string Extra { get; }

    public int RetCode { get; }

    public int Sequence { get; internal set; }
    
    
    
    
    public BotSsoPacket(string command, ReadOnlyMemory<byte> data) : this(command, 0, 0, string.Empty) => Data = data;

    internal BotSsoPacket(string command, ReadOnlyMemory<byte> data, int sequence) : this(command, sequence, 0, string.Empty) => Data = data;

    internal BotSsoPacket(string command, int sequence, int retCode, string extra)
    {
        Command = command;
        Extra = extra;
        RetCode = retCode;
        Sequence = sequence;
    }
}

public enum EncryptType : byte
{
    NoEncrypt = 0x00,
    EncryptD2Key = 0x01,
    EncryptEmpty = 0x02,
}

public enum RequestType
{
    D2Auth = 0x0C,
    Simple = 0x0D,
}