using System.Text.Json.Serialization;
using Lagrange.Core.Events.EventArgs;
using Lagrange.Core.Services;

namespace Lagrange.Core.Common;




[Serializable]
public class BotConfig
{
    
    
    
    public Protocols Protocol { get; set; } = Protocols.Linux;
    
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    
    
    
    public bool AutoReconnect { get; set; } = true;
    
    
    
    
    public bool UseIPv6Network { get; set; } = false;
    
    
    
    
    public bool GetOptimumServer { get; set; } = true;

    
    
    
    public uint HighwayChunkSize { get; set; } = 1024 * 1024;
    
    
    
    
    public uint HighwayConcurrent { get; set; } = 4;
    

    
    
    
    public bool AutoReLogin { get; set; } = true;

    
    
    
    public BotSignProvider? SignProvider { get; set; }

    
    
    
    [JsonIgnore]
    public IReadOnlyList<IService> CustomServices { get; init; } = [];
}




[Flags]
public enum Protocols : byte
{
    None         = 0b00000000,
    
    Windows      = 0b00000001,
    MacOs        = 0b00000010,
    Linux        = 0b00000100,
    AndroidPhone = 0b00001000,
    AndroidPad   = 0b00010000,
    AndroidWatch = 0b00100000,
    
    PC           = Windows | MacOs | Linux,
    Android      = AndroidPhone | AndroidPad | AndroidWatch,
    All          = Windows | MacOs | Linux | AndroidPhone | AndroidPad | AndroidWatch,
}
