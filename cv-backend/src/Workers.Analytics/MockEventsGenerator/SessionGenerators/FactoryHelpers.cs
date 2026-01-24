using Portfolio.Application.Analytics;

namespace Workers.Analytics.MockEventsGenerator.SessionGenerators;

public static class FactoryHelpers
{
    public static ActivityEvent Api(DateTimeOffset ts, string route, int status, Guid sessionId, Guid anonId, int duration = 50) => new()
    {
        Timestamp = ts,
        EventType = "api",
        Route = route,
        Method = "GET",
        StatusCode = status,
        DurationMs = duration,
        SessionId = sessionId,
        AnonymousId = anonId
    };

    public static ActivityEvent PageView(DateTimeOffset ts, string route, Guid sessionId, Guid anonId) => new()
    {
        Timestamp = ts,
        EventType = "page_view",
        Route = route,
        SessionId = sessionId,
        AnonymousId = anonId
    };

    public static ActivityEvent PageLeave(DateTimeOffset ts, string route, Guid sessionId, Guid anonId) => new()
    {
        Timestamp = ts,
        EventType = "page_leave",
        Route = route,
        SessionId = sessionId,
        AnonymousId = anonId
    };

}