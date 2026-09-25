using System.Text.Json.Serialization;

namespace Lagrange.Core.Internal.Packets.Login;

#pragma warning disable CS8618

[Serializable]
internal class NTNewDeviceQrCodeQuery
{
    [JsonPropertyName("uint32_flag")] public long Uint32Flag { get; set; }
    
    [JsonPropertyName("bytes_token")] public string Token { get; set; }
}

[Serializable]
internal class NTNewDeviceQrCodeRequest
{
    [JsonPropertyName("str_dev_auth_token")] public string StrDevAuthToken { get; set; }

    [JsonPropertyName("uint32_flag")] public long Uint32Flag { get; set; }

    [JsonPropertyName("uint32_url_type")] public long Uint32UrlType { get; set; }

    [JsonPropertyName("str_uin_token")] public string StrUinToken { get; set; }

    [JsonPropertyName("str_dev_type")] public string StrDevType { get; set; }

    [JsonPropertyName("str_dev_name")] public string StrDevName { get; set; }
}

public class NTNewDeviceQrCodeResponse
{
    [JsonPropertyName("uint32_guarantee_status")] public long Uint32GuaranteeStatus { get; set; }

    [JsonPropertyName("str_url")] public string StrUrl { get; set; }

    [JsonPropertyName("ActionStatus")] public string ActionStatus { get; set; }
    
    [JsonPropertyName("str_nt_succ_token")] public string StrNtSuccToken { get; set; }
    
    [JsonPropertyName("ErrorCode")] public long ErrorCode { get; set; }

    [JsonPropertyName("ErrorInfo")] public string ErrorInfo { get; set; }
}