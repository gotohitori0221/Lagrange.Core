using Lagrange.Core.Common;

namespace Lagrange.Core.Events.EventArgs;

public class BotRefreshKeystoreEvent(BotKeystore keystore) : EventBase
{
    public BotKeystore Keystore { get; } = keystore;

    public override string ToEventMessage()
    {
        return $"{nameof(BotRefreshKeystoreEvent)}";
    }
}