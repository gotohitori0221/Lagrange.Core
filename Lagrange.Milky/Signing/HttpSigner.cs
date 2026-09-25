using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Lagrange.Core.Common;
using Lagrange.Milky.Configurations;
using Lagrange.Milky.Serialization;

namespace Lagrange.Milky.Signing;

public sealed class HttpSigner : BotSignProvider, IDisposable
{
    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "WhiteListCommand")]
    private static extern ref HashSet<string> GetPCWhiteListCommand([UnsafeAccessorType("Lagrange.Core.Common.DefaultBotSignProvider, Lagrange.Core")] object? _);

    private readonly long _uin;

    private readonly HttpClient _http;

    public HttpSigner(LagrangeConfiguration configuration)
    {
        _uin = configuration.Login.Uin;
        
        _http = new HttpClient(new HttpClientHandler
        {
            Proxy = configuration.Protocol.Signer.ProxyUrl != null
                ? new WebProxy { Address = new Uri(configuration.Protocol.Signer.ProxyUrl) }
                : null
        })
        {
            BaseAddress = new Uri(configuration.Protocol.Signer.NormalizedBaseUrl),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", configuration.Protocol.Signer.Token)
            }
        };
    }

    public override bool IsWhiteListCommand(string cmd) => GetPCWhiteListCommand(null).Contains(cmd);

    public override async Task<SsoSecureInfo?> GetSecSign(long uin, string cmd, int seq, ReadOnlyMemory<byte> body)
    {
        using var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri("sign/sec-sign", UriKind.Relative);
        request.Content = new StringContent(
            Serializer.JsonSerialize(new SecSignRequest
            {
                Uin = uin == 0 ? _uin : uin,
                Command = cmd,
                Sequence = seq,
                Body = Convert.ToHexString(body.Span).ToLower(),
                Guid = Convert.ToHexString(Context.Keystore.Guid).ToLower(),
                Qua = Context.AppInfo.Qua,
            }),
            System.Text.Encoding.UTF8,
            MediaTypeNames.Application.Json
        );

        using var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        var result = await Serializer.JsonDeserializeAsync<SignerResponse<SecSignResult>>(stream);
        if (result == null) throw new Exception("Signer response serialization failed");
        if (result.Code != 0) throw new Exception($"Signer server exception: ({result.Code}){result.Message}");

        return new SsoSecureInfo
        {
            SecSign = Convert.FromHexString(result.Value.SecSign),
            SecToken = Convert.FromHexString(result.Value.SecToken),
            SecExtra = Convert.FromHexString(result.Value.SecExtra),
        };
    }

    
    
    
    
    public static async Task<BotAppInfo?> FetchAppInfoAsync(LagrangeConfiguration configuration, CancellationToken ct = default)
    {
        using var http = new HttpClient(new HttpClientHandler
        {
            Proxy = configuration.Protocol.Signer.ProxyUrl != null
                ? new WebProxy { Address = new Uri(configuration.Protocol.Signer.ProxyUrl) }
                : null
        })
        {
            BaseAddress = new Uri(configuration.Protocol.Signer.NormalizedBaseUrl),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", configuration.Protocol.Signer.Token)
            }
        };

        try
        {
            using var response = await http.GetAsync("sign/sec-sign/appinfo_v2", ct);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            var result = await Serializer.JsonDeserializeAsync<SignerResponse<AppInfoResult>>(stream);
            if (result == null || result.Code != 0) return null;

            var v = result.Value;
            return new BotAppInfo
            {
                Os = v.Os,
                Kernel = v.Kernel,
                VendorOs = v.VendorOs,
                Qua = v.Qua,
                CurrentVersion = v.CurrentVersion,
                PtVersion = v.PtVersion,
                SsoVersion = v.SsoVersion,
                PackageName = v.PackageName,
                ApkSignatureMd5 = Convert.FromHexString(v.ApkSignatureMd5),
                AppId = v.AppId,
                SubAppId = v.SubAppId,
                AppClientVersion = (ushort)v.AppClientVersion,
                SdkInfo = new WtLoginSdkInfo
                {
                    SdkBuildTime = (uint)v.SdkInfo.SdkBuildTime,
                    SdkVersion = v.SdkInfo.SdkVersion,
                    MiscBitMap = (uint)v.SdkInfo.MiscBitMap,
                    SubSigMap = (uint)v.SdkInfo.SubSigMap,
                    MainSigMap = (Sig)v.SdkInfo.MainSigMap,
                }
            };
        }
        catch
        {
            return null;
        }
    }

    public void Dispose()
    {
        _http.Dispose();
    }
}

public class AppInfoResult
{
    [JsonPropertyName("os")] public required string Os { get; init; }
    [JsonPropertyName("kernel")] public required string Kernel { get; init; }
    [JsonPropertyName("vendor_os")] public required string VendorOs { get; init; }
    [JsonPropertyName("qua")] public required string Qua { get; init; }
    [JsonPropertyName("version")] public string? Version { get; init; }
    [JsonPropertyName("current_version")] public required string CurrentVersion { get; init; }
    [JsonPropertyName("pt_version")] public required string PtVersion { get; init; }
    [JsonPropertyName("sso_version")] public required int SsoVersion { get; init; }
    [JsonPropertyName("package_name")] public required string PackageName { get; init; }
    [JsonPropertyName("apk_signature_md5")] public required string ApkSignatureMd5 { get; init; }
    [JsonPropertyName("app_id")] public required int AppId { get; init; }
    [JsonPropertyName("sub_app_id")] public required int SubAppId { get; init; }
    [JsonPropertyName("app_client_version")] public required int AppClientVersion { get; init; }
    [JsonPropertyName("sdk_info")] public required AppInfoSdkInfo SdkInfo { get; init; }
}

public class AppInfoSdkInfo
{
    [JsonPropertyName("sdk_build_time")] public long SdkBuildTime { get; init; }
    [JsonPropertyName("sdk_version")] public required string SdkVersion { get; init; }
    [JsonPropertyName("misc_bit_map")] public uint MiscBitMap { get; init; }
    [JsonPropertyName("sub_sig_map")] public uint SubSigMap { get; init; }
    [JsonPropertyName("main_sig_map")] public long MainSigMap { get; init; }
}

public class SignerResponse<T>
{
    [JsonPropertyName("code")] public required int Code { get; init; }
    [JsonPropertyName("message")] public required string? Message { get; init; }
    [JsonPropertyName("value")] public required T Value { get; init; }
}

public class SecSignRequest
{
    [JsonPropertyName("uin")] public required long Uin { get; init; }
    [JsonPropertyName("command")] public required string Command { get; init; }
    [JsonPropertyName("seq")] public required int Sequence { get; init; }
    [JsonPropertyName("body")] public required string Body { get; init; }
    [JsonPropertyName("guid")] public required string Guid { get; init; }
    [JsonPropertyName("qua")] public required string Qua { get; init; }
}

public class SecSignResult
{
    [JsonPropertyName("sec_sign")] public required string SecSign { get; init; }
    [JsonPropertyName("sec_token")] public required string SecToken { get; init; }
    [JsonPropertyName("sec_extra")] public required string SecExtra { get; init; }
}