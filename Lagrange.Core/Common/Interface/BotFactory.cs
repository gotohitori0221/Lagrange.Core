namespace Lagrange.Core.Common.Interface;

public static class BotFactory
{
    
    
    
    
    
    
    
    public static BotContext Create(BotConfig config, BotKeystore keystore, BotAppInfo? appInfo = null) =>
        new(config, keystore, appInfo ?? BotAppInfo.ProtocolToAppInfo[config.Protocol]);
    
    
    
    
    
    
    
    public static BotContext Create(BotConfig config, BotAppInfo? appInfo = null) =>
        new(config, BotKeystore.CreateEmpty(), appInfo ?? BotAppInfo.ProtocolToAppInfo[config.Protocol]);
}