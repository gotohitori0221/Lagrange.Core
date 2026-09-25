using Lagrange.Core.Common;
using Lagrange.Core.Internal.Packets.Login;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Cryptography;
using Lagrange.Proto;

namespace Lagrange.Core.Internal.Services.Login;

internal static class NTLoginCommon
{
    private const string Tag = nameof(NTLoginCommon);
    
    public static ReadOnlyMemory<byte> Encode<T>(BotContext context, T body) where T : IProtoSerializable<T>
    {
        if (context.Keystore.State.KeyExchangeSession is not { } session)
        {
            context.LogError(Tag, "Key exchange session is not initialized.");
            throw new InvalidOperationException("Key exchange session is not initialized.");
        }

        var login = BuildCommon(context, body);
        var forward = new NTLoginForwardRequest
        {
            SessionTicket = session.SessionTicket,
            Buffer = AesGcmProvider.Encrypt(ProtoHelper.Serialize(login).Span, session.SessionKey),
            Type = 1
        };

        return ProtoHelper.Serialize(forward);
    }
    
    public static ReadOnlyMemory<byte> EncodeAndroid<T>(BotContext context, T body) where T : IProtoSerializable<T>
    {
        if (context.Keystore.State.KeyExchangeSession is not { } session)
        {
            context.LogError(Tag, "Key exchange session is not initialized.");
            throw new InvalidOperationException("Key exchange session is not initialized.");
        }

        var login = new NTLoginAndroidCommon
        {
            Common = BuildCommon(context, body),
            Ext = new NTLoginAndroidExt { Field1 = 0, Uin = context.Keystore.Uin.ToString() }
        };
        var forward = new NTLoginForwardRequest
        {
            SessionTicket = session.SessionTicket,
            Type = 1,
            SecBuffer = AesGcmProvider.Encrypt(ProtoHelper.Serialize(login).Span, session.SessionKey),
        };

        return ProtoHelper.Serialize(forward);
    }

    public static NTLoginRetCode Decode<T>(BotContext context, ReadOnlyMemory<byte> payload, out NTLoginErrorInfo? info, out T resp) where T : IProtoSerializable<T>
    {
        if (context.Keystore.State.KeyExchangeSession is not { } session)
        {
            context.LogError(Tag, "Key exchange session is not initialized.");
            throw new InvalidOperationException("Key exchange session is not initialized.");
        }
        
        var forward = ProtoHelper.Deserialize<NTLoginForwardRequest>(payload.Span);
        var buffer = AesGcmProvider.Decrypt(forward.Buffer, session.SessionKey);

        var login = ProtoHelper.Deserialize<Packets.Login.NTLoginCommon>(buffer);
        if (login.Head.ErrorInfo.ErrCode != 0)
        {
            info = login.Head.ErrorInfo;
            resp = ProtoHelper.Deserialize<T>(login.Body.Span);
            return (NTLoginRetCode)login.Head.ErrorInfo.ErrCode;
        }

        info = null;
        resp = ProtoHelper.Deserialize<T>(login.Body.Span);
        return NTLoginRetCode.LOGIN_SUCCESS;
    }
    
    public static NTLoginRetCode DecodeAndroid<T>(BotContext context, ReadOnlyMemory<byte> payload, out NTLoginHead? head, out T resp) where T : IProtoSerializable<T>
    {
        if (context.Keystore.State.KeyExchangeSession is not { } session)
        {
            context.LogError(Tag, "Key exchange session is not initialized.");
            throw new InvalidOperationException("Key exchange session is not initialized.");
        }
        
        var forward = ProtoHelper.Deserialize<NTLoginForwardRequest>(payload.Span);
        var buffer = AesGcmProvider.Decrypt(forward.SecBuffer, session.SessionKey);
        var login = ProtoHelper.Deserialize<NTLoginAndroidCommon>(buffer).Common;

        if (login.Head.Cookie != null) context.Keystore.State.Cookie = login.Head.Cookie.CookieContent;
        
        if (login.Head.ErrorInfo.ErrCode != 0)
        {
            head = login.Head;
            resp = ProtoHelper.Deserialize<T>(login.Body.Span);
            return (NTLoginRetCode)login.Head.ErrorInfo.ErrCode;
        }

        head = null;
        resp = ProtoHelper.Deserialize<T>(login.Body.Span);
        return NTLoginRetCode.LOGIN_SUCCESS;
    }
    
    private static Packets.Login.NTLoginCommon BuildCommon<T>(BotContext context, T body) where T : IProtoSerializable<T> => new()
    {
        Head = new NTLoginHead
        {
            UserInfo = new NTLoginUserInfo { Account = context.Keystore.Uin.ToString() },
            ClientInfo = new NTLoginClientInfo
            {
                DeviceType = context.AppInfo.Os,
                DeviceName = context.Keystore.DeviceName,
                Platform = context.Config.Protocol switch
                {
                    Protocols.Windows => NTLoginPlatform.PLATFORM_WINDOWS,
                    Protocols.MacOs => NTLoginPlatform.PLATFORM_MAC,
                    Protocols.Linux => NTLoginPlatform.PLATFORM_LINUX,
                    Protocols.AndroidPad or Protocols.AndroidPhone => NTLoginPlatform.PLATFORM_ANDROID,
                    _ => NTLoginPlatform.PLATFORM_UNKNOWN
                },
                Guid = context.Keystore.Guid
            },
            AppInfo = new NTLoginAppInfo
            {
                Version = context.AppInfo.Kernel,
                AppId = context.AppInfo.AppId,
                AppName = context.AppInfo.PackageName,
                Qua = context.AppInfo.Qua,
            },
            SdkInfo = new NTLoginSdkInfo { Version = 1 },
            Cookie = context.Keystore.State.Cookie is { } cookie ? new NTLoginCookie { CookieContent = cookie } : new NTLoginCookie(),
        },
        Body = ProtoHelper.Serialize(body)
    };
    
    public static void SaveTicket(BotContext context, NTLoginTickets tickets)
    {
        var sigs = context.Keystore.WLoginSigs;
        
        sigs.A1 = tickets.A1;
        sigs.A2 = tickets.A2;
        sigs.D2 = tickets.D2;
        sigs.D2Key = tickets.D2Key;
    }
}