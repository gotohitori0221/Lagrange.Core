namespace Lagrange.Core.Utility.Extension;

internal static class CollectionExt
{
    public static TValue? GetOrNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) => 
        dictionary.TryGetValue(key, out var value) ? value : default;
}