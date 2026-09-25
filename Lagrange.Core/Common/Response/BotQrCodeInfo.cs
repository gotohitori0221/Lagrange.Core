using System.Text.Json.Serialization;

namespace Lagrange.Core.Common.Response;

[Serializable]
public class BotQrCodeInfo(string message, string platform, string location, string? device)
{
    [JsonPropertyName("message")] public string Message { get; } = message;
    
    [JsonPropertyName("platform")] public string Platform { get; } = platform;
    
    [JsonPropertyName("location")] public string Location { get; } = location;
    
    [JsonPropertyName("device")] public string? Device { get; } = device;
}