using Lagrange.Core.Internal.Packets.Message;

namespace Lagrange.Core.Message.Entities;

public interface IMessageEntity
{
    internal Task Preprocess(BotContext context, BotMessage message) => Task.CompletedTask;
    
    internal Task Postprocess(BotContext context, BotMessage message) => Task.CompletedTask;

    internal Elem[] Build();

    internal IMessageEntity? Parse(List<Elem> elements, Elem target);

    internal string ToPreviewString();
}