using Lagrange.Core.Events;

namespace Lagrange.Core.Services;




public interface IService
{
    ValueTask<ProtocolEvent> Parse(ReadOnlyMemory<byte> input, BotContext context);

    ValueTask<ReadOnlyMemory<byte>> Build(ProtocolEvent input, BotContext context);
}
