using System.Threading.Tasks.Sources;
using Lagrange.Core.Common.Entity;

namespace Lagrange.Core.Internal.Packets.Struct;

internal class SsoPacketValueTaskSource : IValueTaskSource<BotSsoPacket>
{
    private ManualResetValueTaskSourceCore<BotSsoPacket> _core = new()
    {
        RunContinuationsAsynchronously = true,
    };

    public BotSsoPacket GetResult(short token) => _core.GetResult(token);

    public ValueTaskSourceStatus GetStatus(short token) => _core.GetStatus(token);

    public void OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags) => _core.OnCompleted(continuation, state, token, flags);

    public void SetResult(BotSsoPacket result) => _core.SetResult(result);

    public void SetException(Exception exception) => _core.SetException(exception);
}