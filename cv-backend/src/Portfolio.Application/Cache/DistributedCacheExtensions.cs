using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Portfolio.Application.Cache;

public static class DistributedCacheExtensions
{
    private static readonly JsonSerializerOptions JsonOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static async Task<T?> GetOrCreateAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T>> factory,
        TimeSpan ttl)
    {
        var cached = await cache.GetStringAsync(key);
        if (cached != null)
            return JsonSerializer.Deserialize<T>(cached, JsonOptions);

        var value = await factory();
        if (value == null)
            return default;

        await cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(value, JsonOptions),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            });

        return value;
    }
}