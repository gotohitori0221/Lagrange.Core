using Lagrange.Core.Common;

namespace Lagrange.Core.Internal.Packets.Struct;

internal abstract class StructBase(BotContext context)
{
    protected BotKeystore Keystore => context.Keystore;
    
    protected BotAppInfo AppInfo => context.AppInfo;
}