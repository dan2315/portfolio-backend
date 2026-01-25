using Portfolio.Application.Analytics;
using Portfolio.Application.Analytics.DTOs;
using Portfolio.Infrastructure.Analytics.Interfaces;
using StackExchange.Redis;
using Workers.Analytics;

namespace Portfolio.Infrastructure.Analytics;

public class LiveSessionsStore : ILiveSessionsStore
{
    private const string storeKey = "live:session";
    private readonly IDatabase _redisDb;
    private readonly TimeSpan SessionCacheTtl = TimeSpan.FromHours(8);

    public LiveSessionsStore(IConnectionMultiplexer redis)
    {
        _redisDb = redis.GetDatabase();
    }

    public async Task<IReadOnlyList<SessionDTO>> GetSessions()
    {
        var to = DateTime.UtcNow;
        var from = DateTime.UtcNow.AddHours(-8);
        return await GetSessions(from, to);
    }
    public async Task<IReadOnlyList<SessionDTO>> GetSessions(DateTimeOffset from, DateTimeOffset to)
    {
        var sessions = new List<SessionDTO>();

        var globalKey = $"{storeKey}:all";
        double minScore = from.ToUnixTimeMilliseconds();
        double maxScore = to.ToUnixTimeMilliseconds();

        var sessionKeys = await _redisDb.SortedSetRangeByScoreAsync(globalKey, minScore, maxScore);

        foreach (var sessionKey in sessionKeys)
        {
            var parts = sessionKey.ToString().Split(':');
            var userId = Guid.Parse(parts[0]);
            var sessionId = Guid.Parse(parts[1]);

            var key = $"live:session:{userId}:{sessionId}";
            var hash = await _redisDb.HashGetAllAsync(key);
            if (hash.Length == 0) continue;

            var sessionDict = hash.ToDictionary(h => h.Name.ToString(), h => h.Value.ToString());
            var session = ActivitySessionMapper.FromRedisHash(sessionId, sessionDict);
            sessions.Add(session);
        }

        return sessions;
    }

    public async Task<IReadOnlyList<SessionDTO>> GetSessions(Guid userId, int limit)
    {
        var sessions = new List<SessionDTO>();

        var userSetKey = $"{storeKey}:{userId}";
        var sessionIds = await _redisDb.SetMembersAsync(userSetKey);

        foreach (var sessionIdValue in sessionIds.Take(limit))
        {
            var sessionId = sessionIdValue.ToString();
            RedisKey key = $"{storeKey}:{userId}:{sessionId}";

            var hash = await _redisDb.HashGetAllAsync(key);
            var sessionDictHast = hash.ToDictionary(h => h.Name.ToString(), h => h.Value.ToString());
            var session = ActivitySessionMapper.FromRedisHash(userId, sessionDictHast);
            sessions.Add(session);
        }

        return sessions;
    }

    public async Task StoreSessions(SessionDeltaState state)
    {
        var globalZSetKey = $"{storeKey}:all";
        var key = $"{storeKey}:{state.AnonymousId}:{state.SessionId}";
        var userSetKey = $"{storeKey}:{state.AnonymousId}";

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
            batch.KeyExpireAsync(key, SessionCacheTtl),

            batch.SetAddAsync(userSetKey, state.SessionId.ToString()),
            batch.KeyExpireAsync(userSetKey, SessionCacheTtl),
            
            batch.SortedSetAddAsync(globalZSetKey, $"{state.AnonymousId}:{state.SessionId}", state.LeastStartTime.ToUnixTimeMilliseconds()),
            batch.KeyExpireAsync(globalZSetKey, SessionCacheTtl)
        };

        batch.Execute();

        await Task.WhenAll(tasks);
    }
}