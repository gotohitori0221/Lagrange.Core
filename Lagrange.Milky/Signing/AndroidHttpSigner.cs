using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Threading;
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

    private readonly long _uin;
    private readonly HttpClient _http;

    public AndroidHttpSigner(LagrangeConfiguration configuration)
    {
        _uin = configuration.Login.Uin;
        var signer = configuration.Protocol.AndroidSigner;

        _http = new HttpClient(new HttpClientHandler
        {
            Proxy = signer.ProxyUrl != null
                ? new WebProxy { Address = new Uri(signer.ProxyUrl) }
                : null
        })
        {
            BaseAddress = new Uri(signer.NormalizedBaseUrl),
        };

        if (!string.IsNullOrEmpty(signer.Token))
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", signer.Token);
        }
    }

    private string Qua => string.IsNullOrEmpty(Context.AppInfo.Qua) ? DefaultQua : Context.AppInfo.Qua;

    public override bool IsWhiteListCommand(string cmd) => GetAndroidWhiteListCommand(null).Contains(cmd);

    public override async Task<SsoSecureInfo?> GetSecSign(long uin, string cmd, int seq, ReadOnlyMemory<byte> body)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "sign/sec-sign")
        {
            Content = new StringContent(
                Serializer.JsonSerialize(new SecSignRequest
                {
                    Uin = uin == 0 ? _uin : uin,
                    Command = cmd,
                    Sequence = seq,
                    Body = Convert.ToHexString(body.Span).ToLower(),
                    Guid = Convert.ToHexString(Context.Keystore.Guid).ToLower(),
                    Qua = Qua,
                }),
                System.Text.Encoding.UTF8,
                MediaTypeNames.Application.Json
            )
        };

        using var response = await _http.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        using var stream = await response.Content.ReadAsStreamAsync();
        var result = await Serializer.JsonDeserializeAsync<SignerResponse<SecSignResult>>(stream);
        if (result == null || result.Code != 0) return null;

        return new SsoSecureInfo
        {
            SecSign = FromHexOrEmpty(result.Value.SecSign),
            SecToken = FromHexOrEmpty(result.Value.SecToken),
            SecExtra = FromHexOrEmpty(result.Value.SecExtra)
        };
    }

    public static async Task<BotAppInfo?> FetchAppInfoAsync(LagrangeConfiguration configuration, CancellationToken ct = default)
    {
        var signer = configuration.Protocol.AndroidSigner;
        if (string.IsNullOrEmpty(signer.BaseUrl)) return null;

        using var http = new HttpClient(new HttpClientHandler
        {
            Proxy = signer.ProxyUrl != null
                ? new WebProxy { Address = new Uri(signer.ProxyUrl) }
                : null
        })
        {
            BaseAddress = new Uri(signer.NormalizedBaseUrl)
        };

        if (!string.IsNullOrEmpty(signer.Token))
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", signer.Token);
        }

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
                ApkSignatureMd5 = FromHexOrEmpty(v.ApkSignatureMd5),
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

    public override async Task<byte[]> GetEnergy(long uin, string data)
    {
        try
        {
            var payload = new JsonObject
            {
                ["uin"] = uin == 0 ? _uin : uin,
                ["data"] = data,
                ["guid"] = Convert.ToHexString(Context.Keystore.Guid).ToLower(),
                ["ver"] = Context.AppInfo.SdkInfo.SdkVersion,
                ["version"] = Context.AppInfo.PtVersion,
                ["qua"] = Qua
            };

            using var response = await _http.PostAsync("energy", new StringContent(payload.ToJsonString(), System.Text.Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode) return [];

            var str = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(str);
            var val = node?["data"]?.ToString() ?? node?["value"]?.ToString();
            return FromHexOrEmpty(val);
        }
        catch
        {
            return [];
        }
    }

    public override async Task<byte[]> GetDebugXwid(long uin, string data)
    {
        try
        {
            var payload = new JsonObject
            {
                ["uin"] = uin == 0 ? _uin : uin,
                ["data"] = data,
                ["guid"] = Convert.ToHexString(Context.Keystore.Guid).ToLower(),
                ["version"] = Context.AppInfo.PtVersion,
                ["qua"] = Qua
            };

            using var response = await _http.PostAsync("get_tlv553", new StringContent(payload.ToJsonString(), System.Text.Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode) return [];

            var str = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(str);
            var val = node?["data"]?.ToString() ?? node?["value"]?.ToString();
            return FromHexOrEmpty(val);
        }
        catch
        {
            return [];
        }
    }

    private static byte[] FromHexOrEmpty(string? hex)
    {
        if (string.IsNullOrEmpty(hex)) return [];
        try { return Convert.FromHexString(hex); }
        catch { return []; }
    }

    public void Dispose()
    {
        _http.Dispose();
    }
}
