using System.Collections.Concurrent;

namespace Lagrange.Proto.Serialization.Metadata;

internal sealed partial class ReflectionEmitCachingMemberAccessor
{
    private sealed class Cache<TKey>(TimeSpan slidingExpiration, TimeSpan evictionInterval) where TKey : notnull
    {
        private int _evictLock;
        private long _lastEvictedTicks = DateTime.UtcNow.Ticks; 
        private readonly long _evictionIntervalTicks = evictionInterval.Ticks; 
        private readonly long _slidingExpirationTicks = slidingExpiration.Ticks; 
        private readonly ConcurrentDictionary<TKey, CacheEntry> _cache = new();

        public TValue GetOrAdd<TValue>(TKey key, Func<TKey, TValue> valueFactory) where TValue : class?
        {
            var entry = _cache.GetOrAdd(key, static (key, valueFactory) => new CacheEntry(valueFactory(key)), valueFactory);
            long utcNowTicks = DateTime.UtcNow.Ticks;
            Volatile.Write(ref entry.LastUsedTicks, utcNowTicks);

            if (utcNowTicks - Volatile.Read(ref _lastEvictedTicks) >= _evictionIntervalTicks)
            {
                if (Interlocked.CompareExchange(ref _evictLock, 1, 0) == 0)
                {
                    if (utcNowTicks - _lastEvictedTicks >= _evictionIntervalTicks)
                    {
                        EvictStaleCacheEntries(utcNowTicks);
                        Volatile.Write(ref _lastEvictedTicks, utcNowTicks);
                    }

                    Volatile.Write(ref _evictLock, 0);
                }
            }

            return (TValue)entry.Value!;
        }

        public void Clear()
        {
            _cache.Clear();
            _lastEvictedTicks = DateTime.UtcNow.Ticks;
        }

        private void EvictStaleCacheEntries(long utcNowTicks)
        {
            foreach (var kvp in _cache)
            {
                if (utcNowTicks - Volatile.Read(ref kvp.Value.LastUsedTicks) >= _slidingExpirationTicks)
                {
                    _cache.TryRemove(kvp.Key, out _);
                }
            }
        }

        private sealed class CacheEntry(object? value)
        {
            public readonly object? Value = value;
            public long LastUsedTicks;
        }
    }
}
