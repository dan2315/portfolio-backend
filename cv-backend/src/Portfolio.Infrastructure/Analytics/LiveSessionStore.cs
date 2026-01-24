using Portfolio.Application.Analytics;
using Portfolio.Application.Analytics.DTOs;
using StackExchange.Redis;
using Workers.Analytics;

namespace Portfolio.Infrastructure.Analytics;

public class LiveSessionsStore
{
    private const string storeKey = "live:session";
    private readonly IDatabase _redisDb;
    private readonly TimeSpan SessionCacheTtl = TimeSpan.FromHours(8);

    public LiveSessionsStore(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task<IReadOnlyList<SessionDTO>> GetSessions(int limit)
    {
        var keys = await _redisDb.SetMembersAsync(storeKey);
        var sessions = new List<SessionDTO>();

        foreach (var keyValue in keys.Take(limit))
        {
            RedisKey key = keyValue.ToString();
            var hash = await _redisDb.HashGetAllAsync(key);
            var sessionDictHast = hash.ToDictionary(h => h.Name.ToString(), h => h.Value.ToString());
            var session = ActivitySessionMapper.FromRedisHash(new Guid(), sessionDictHast);
            sessions.Add(session);
        }

        return sessions;
    }

    public async Task StoreSessions(SessionDeltaState state)
    {
        var key = $"{storeKey}:{state.SessionId}";

        var batch = _redisDb.CreateBatch();

        var tasks = new List<Task>
        {
            batch.HashIncrementAsync(key, "pagesViewed", state.PagesViewed),
            batch.HashIncrementAsync(key, "cartridgesInserted", state.CartridgesInserted),
            batch.HashIncrementAsync(key, "contactAttempted", state.ContactAttempts),
            batch.HashSetAsync(key,
            [
                new HashEntry("start", state.LeastStartTime.ToUnixTimeMilliseconds()),
                new HashEntry("end", state.GreatestEndTime.ToUnixTimeMilliseconds()),
                new HashEntry("totalTimeMs", state.TotalTimeMs)
            ]),
            batch.KeyExpireAsync(key, SessionCacheTtl)
        };

        batch.Execute();

        await Task.WhenAll(tasks);
    }
}