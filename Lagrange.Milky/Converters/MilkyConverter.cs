using Lagrange.Core;
using Lagrange.Milky.Caching;
using Lagrange.Milky.Storage;

namespace Lagrange.Milky.Converters;

public partial class MilkyConverter(BotContext lagrange, MessageCache cache, MessageStore store, ResourceConverter resourceConverter)
{
    private readonly BotContext _lagrange = lagrange;
    private readonly MessageCache _cache = cache;
    private readonly MessageStore _store = store;
    private readonly ResourceConverter _resourceConverter = resourceConverter;
}

