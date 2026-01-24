using Portfolio.Application.Analytics;

namespace Workers.Analytics.MockEventsGenerator.SessionGenerators;

public sealed class FastNavigationSessionGenerator : ISessionPatternGenerator
{
    private static readonly string[] Pages =
    {
    "/",
    "/projects",
    "/projects/1",
    "/experience",
    "/contact"
};

    public IEnumerable<ActivityEvent> Generate(
        DateTimeOffset start,
        Guid sessionId,
        Guid anonId)
    {
        var ts = start;

        foreach (var page in Pages)
        {
            yield return new ActivityEvent
            {
                Timestamp = ts,
                EventType = "page_view",
                Route = page,
                SessionId = sessionId,
                AnonymousId = anonId,
                TimeOnPageMs = Random.Shared.Next(20, 70)
            };

            ts += TimeSpan.FromMilliseconds(Random.Shared.Next(20, 70));

            yield return FactoryHelpers.PageLeave(ts, page, sessionId, anonId);
            ts += TimeSpan.FromMilliseconds(10);
        }
    }
}