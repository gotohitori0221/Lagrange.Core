using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Lagrange.Core.Internal.Packets.Service;

#pragma warning disable CS8618

[Serializable]
public class LightApp
{
    [JsonPropertyName("app")] public string App { get; set; }

    [JsonPropertyName("config")] public Config Config { get; set; }

    [JsonPropertyName("desc")] public string Desc { get; set; }

    [JsonPropertyName("from")] public long From { get; set; }

    [JsonPropertyName("meta")] public JsonObject Meta { get; set; }
    
    [JsonPropertyName("extra")] public string? Extra { get; set; }

    [JsonPropertyName("prompt")] public string Prompt { get; set; }

    [JsonPropertyName("ver")] public string Ver { get; set; }

    [JsonPropertyName("view")] public string View { get; set; }
    
    [JsonPropertyName("bizsrc")] public string BizSrc { get; set; }
}

[Serializable]
public class Config
{
    [JsonPropertyName("autosize")] public int Autosize { get; set; }
    
    
    
    [JsonPropertyName("token")] public string Token { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }
    
    [JsonPropertyName("forward")] public int Forward { get; set; }
    
    [JsonPropertyName("round")] public int Round { get; set; }
    
    
    
    [JsonPropertyName("width")] public int Width { get; set; }
    
    
}