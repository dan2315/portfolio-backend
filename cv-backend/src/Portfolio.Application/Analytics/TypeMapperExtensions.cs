using Portfolio.Application.Analytics.DTOs;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Analytics;

public static class ActivitySessionMapper
{
    public static SessionDTO MapToDTO(this ActivitySession session)
    {
        return new SessionDTO(
            session.SessionId,
            session.StartTime,
            session.EndTime,
            session.PagesViewed,
            session.CartridgesInserted,
            session.ContactAttempted,
            session.TotalTimeMs,
            session.AdditionalData
        );
    }
    public static SessionDTO FromRedisHash(Guid sessionId, IReadOnlyDictionary<string, string> hash)
    {
        return new SessionDTO(
            sessionId,
            FromUnixMs(hash, "start"),
            FromUnixMs(hash, "end"),
            GetInt(hash, "pagesViewed"),
            GetInt(hash, "cartridgesInserted"),
            GetInt(hash, "contactAttempted"),
            GetLong(hash, "totalTimeMs"),
            null
        );
    }

    private static DateTimeOffset FromUnixMs(IReadOnlyDictionary<string, string> dict, string key)
        => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(dict[key]));

    private static int GetInt(IReadOnlyDictionary<string, string> dict, string key)
        => dict.TryGetValue(key, out var v) ? int.Parse(v) : 0;

    private static long GetLong(IReadOnlyDictionary<string, string> dict, string key)
        => dict.TryGetValue(key, out var v) ? long.Parse(v) : 0;
}
