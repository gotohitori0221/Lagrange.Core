using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Lagrange.Core.Common;
using Lagrange.Milky.Configurations;
using Lagrange.Milky.Serialization;

namespace Lagrange.Milky.Signing;

public sealed class AndroidHttpSigner : AndroidBotSignProvider, IDisposable
{
    private const string DefaultQua = "V1_AND_SQ_9.2.20_11650_YYB_D";

    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "WhiteListCommand")]
    private static extern ref HashSet<string> GetAndroidWhiteListCommand([UnsafeAccessorType("Lagrange.Core.Common.DefaultAndroidBotSignProvider, Lagrange.Core")] object? _);

    private readonly HttpClient _http;

    public AndroidHttpSigner(LagrangeConfiguration configuration)
    {
        var signer = configuration.Protocol.AndroidSigner;

        _http = new HttpClient(new HttpClientHandler
        {
            Proxy = signer.ProxyUrl != null
                ? new WebProxy { Address = new Uri(signer.ProxyUrl) }
                : null
        })
        {
            BaseAddress = new Uri(signer.NormalizedBaseUrl),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", signer.Token)
            }
        };
    }

    private string Qua => string.IsNullOrEmpty(Context.AppInfo.Qua) ? DefaultQua : Context.AppInfo.Qua;

    public override bool IsWhiteListCommand(string cmd) => GetAndroidWhiteListCommand(null).Contains(cmd);

    public override async Task<SsoSecureInfo?> GetSecSign(long uin, string cmd, int seq, ReadOnlyMemory<byte> body)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "sign")
        {
            Content = new StringContent(
                Serializer.JsonSerialize(new AndroidSecSignRequest
                {
                    Uin = uin,
                    Cmd = cmd,
                    Seq = seq,
                    Buffer = Convert.ToHexString(body.Span),
                    Guid = Convert.ToHexString(Context.Keystore.Guid),
                    Version = Context.AppInfo.PtVersion,
                    Qua = Qua
                }),
                Encoding.UTF8,
                "application/json"
            )
        };

        using var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        using var stream = await response.Content.ReadAsStreamAsync();
        var result = await Serializer.JsonDeserializeAsync<AndroidSignerResponse<AndroidSignResult>>(stream);
        if (result?.Data == null) return null;

        return new SsoSecureInfo
        {
            SecSign = FromHexOrEmpty(result.Data.Sign),
            SecToken = FromHexOrEmpty(result.Data.Token),
            SecExtra = FromHexOrEmpty(result.Data.Extra)
        };
    }

    public override Task<byte[]> GetEnergy(long uin, string data)
        => PostForHex("energy", new AndroidEnergyRequest
        {
            Uin = uin,
            Data = data,
            Guid = Convert.ToHexString(Context.Keystore.Guid),
            Ver = Context.AppInfo.SdkInfo.SdkVersion,
            Version = Context.AppInfo.PtVersion,
            Qua = Qua
        });

    public override Task<byte[]> GetDebugXwid(long uin, string data)
        => PostForHex("get_tlv553", new AndroidDebugXwidRequest
        {
            Uin = uin,
            Data = data,
            Guid = Convert.ToHexString(Context.Keystore.Guid),
            Version = Context.AppInfo.PtVersion,
            Qua = Qua
        });

    private async Task<byte[]> PostForHex<T>(string path, T payload)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = new StringContent(Serializer.JsonSerialize(payload), Encoding.UTF8, "application/json")
        };

        using var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return [];

        using var stream = await response.Content.ReadAsStreamAsync();
        var result = await Serializer.JsonDeserializeAsync<AndroidSignerResponse<string>>(stream);
        return FromHexOrEmpty(result?.Data);
    }

    private static byte[] FromHexOrEmpty(string? hex)
        => string.IsNullOrEmpty(hex) ? [] : Convert.FromHexString(hex);

    public void Dispose()
    {
        _http.Dispose();
    }
}

public class AndroidSecSignRequest
{
    [JsonPropertyName("uin")] public required long Uin { get; init; }
    [JsonPropertyName("cmd")] public required string Cmd { get; init; }
    [JsonPropertyName("seq")] public required int Seq { get; init; }
    [JsonPropertyName("buffer")] public required string Buffer { get; init; }
    [JsonPropertyName("guid")] public required string Guid { get; init; }
    [JsonPropertyName("version")] public required string Version { get; init; }
    [JsonPropertyName("qua")] public required string Qua { get; init; }
}

public class AndroidEnergyRequest
{
    [JsonPropertyName("uin")] public required long Uin { get; init; }
    [JsonPropertyName("data")] public required string Data { get; init; }
    [JsonPropertyName("guid")] public required string Guid { get; init; }
    [JsonPropertyName("ver")] public required string Ver { get; init; }
    [JsonPropertyName("version")] public required string Version { get; init; }
    [JsonPropertyName("qua")] public required string Qua { get; init; }
}

public class AndroidDebugXwidRequest
{
    [JsonPropertyName("uin")] public required long Uin { get; init; }
    [JsonPropertyName("data")] public required string Data { get; init; }
    [JsonPropertyName("guid")] public required string Guid { get; init; }
    [JsonPropertyName("version")] public required string Version { get; init; }
    [JsonPropertyName("qua")] public required string Qua { get; init; }
}

public class AndroidSignerResponse<T>
{
    [JsonPropertyName("data")] public required T Data { get; init; }
}

public class AndroidSignResult
{
    [JsonPropertyName("sign")] public required string Sign { get; init; }
    [JsonPropertyName("token")] public required string Token { get; init; }
    [JsonPropertyName("extra")] public required string Extra { get; init; }
}
