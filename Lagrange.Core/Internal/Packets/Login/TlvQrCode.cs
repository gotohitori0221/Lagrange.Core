using Lagrange.Core.Common;
using Lagrange.Core.Utility;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Packets.Login;

internal ref struct TlvQrCode : IDisposable
{
    private BinaryPacket _writer;

    private short _count;
    
    private readonly BotKeystore _keystore;
    
    private readonly BotAppInfo _appInfo;

    public TlvQrCode(BotContext context)
    {
        _keystore = context.Keystore;
        _appInfo = context.AppInfo;
        
        _writer = new BinaryPacket(300);
        _writer.Skip(2);
    }
    
    public void Tlv02()
    {
        WriteTlv(0x2);

        _writer.Write(0);
        _writer.Write(0x0B);
        
        _writer.ExitLengthBarrier<short>(false);
    }

    public void Tlv04()
    {
        WriteTlv(0x4);
        
        _writer.Write<short>(0x00); 
        _writer.Write(_keystore.Uin.ToString(), Prefix.Int16 | Prefix.LengthOnly);
        
        _writer.ExitLengthBarrier<short>(false);
    }
    
    public void Tlv09()
    {
        WriteTlv(0x09);
        
        _writer.Write(_appInfo.PackageName);
        
        _writer.ExitLengthBarrier<short>(false);
    }
    
    public void Tlv11(byte[] unusualSig)
    {
        WriteTlv(0x11);
        
        _writer.Write(unusualSig);
        
        _writer.ExitLengthBarrier<short>(false);
    }

    public void Tlv15()
    {
        WriteTlv(0x15);

        _writer.Write(0);
        
        _writer.ExitLengthBarrier<short>(false);
    }
    
    public void Tlv16()
    {
        WriteTlv(0x16);
        
        _writer.Write(0u);
        _writer.Write(_appInfo.AppId);
        _writer.Write(_appInfo.SubAppId);
        _writer.Write(_keystore.Guid);
        _writer.Write(_appInfo.PackageName, Prefix.Int16 | Prefix.LengthOnly);
        _writer.Write(_appInfo.PtVersion, Prefix.Int16 | Prefix.LengthOnly);
        _writer.Write(_appInfo.PackageName, Prefix.Int16 | Prefix.LengthOnly);
        
        _writer.ExitLengthBarrier<short>(false);
    }
    
    public void Tlv18()
    {
        WriteTlv(0x18);
        
        _writer.Write(_keystore.WLoginSigs.A1);
        
        _writer.ExitLengthBarrier<short>(false);
    }
    
    public void Tlv19()
    {
        WriteTlv(0x19);
        
        _writer.Write(_keystore.WLoginSigs.NoPicSig);
        
        _writer.ExitLengthBarrier<short>(false);
    }

    public void Tlv1B()
    {
        WriteTlv(0x1B);
        
        _writer.Write(0u); 
        _writer.Write(0u); 
        _writer.Write(3u); 
        _writer.Write(4u); 
        _writer.Write(72u); 
        _writer.Write(2u); 
        _writer.Write(2u); 
        _writer.Write((ushort)0u); 
        
        _writer.ExitLengthBarrier<short>(false);
    }
    
    public void Tlv1D()
    {
        WriteTlv(0x1D);
        
        _writer.Write((byte)1u);
        _writer.Write(_appInfo.SdkInfo.MiscBitMap);
        _writer.Write(0u);
        _writer.Write((byte)0);
        
        _writer.ExitLengthBarrier<short>(false);
    }

    public void Tlv33()
    {
        WriteTlv(0x33);
        
        _writer.Write(_keystore.Guid);
        
        _writer.ExitLengthBarrier<short>(false);
    }
    
    public void Tlv35()
    {
        WriteTlv(0x35);
        
        _writer.Write(_appInfo.SsoVersion);
        
        _writer.ExitLengthBarrier<short>(false);
    }
    
    public void Tlv39()
    {
        WriteTlv(0x39);
        
        _writer.Write(0x01);
        
        _writer.ExitLengthBarrier<short>(false);
    }
    
    public void Tlv66()
    {
        WriteTlv(0x66);
        
        _writer.Write(_appInfo.SsoVersion);
        
        _writer.ExitLengthBarrier<short>(false);
    }

    public void Tlv68()
    {
        WriteTlv(0x68);
        
        _writer.Write(_keystore.Guid);
        
        _writer.ExitLengthBarrier<short>(false);
    }

    public void TlvD1()
    {
        WriteTlv(0xD1);
        
        var obj = new QrExtInfo
        {
            DevInfo = new DevInfo
            {
                DevType = _appInfo.Os,
                DevName = _keystore.DeviceName
            },
            GenInfo = new GenInfo
            {
                Field6 = 1
            }
        };
        ProtoHelper.Serialize(ref _writer, obj);
        
        _writer.ExitLengthBarrier<short>(false);
    }

    public void Tlv12C()
    {
        WriteTlv(0x12C);
        
        var obj = new ScanExtInfo
        {
            Guid = _keystore.Guid,
            Imei = _keystore.Qimei,
            ScanScene = 1,
            AllowAutoRenewTicket = true
        };
        ProtoHelper.Serialize(ref _writer, obj);
        
        _writer.ExitLengthBarrier<short>(false);
    }

    public ReadOnlySpan<byte> CreateReadOnlySpan()
    {
        _writer.Write(_count, 0);
        return _writer.CreateReadOnlySpan();
    }
    
    private void WriteTlv(short tag)
    {
        _writer.Write(tag);
        _writer.EnterLengthBarrier<short>();
        _count++;
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}