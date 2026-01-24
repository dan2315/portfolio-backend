using Portfolio.Application.Analytics;
using Workers.Analytics.MockEventsGenerator.SessionGenerators;

public sealed class ApiFloodSessionGenerator : ISessionPatternGenerator
{
    public IEnumerable<ActivityEvent> Generate(
        DateTimeOffset start,
        Guid sessionId,
        Guid anonId)
    {
        var ts = start;

        for (int i = 0; i < Random.Shared.Next(100, 500); i++)
        {
            yield return FactoryHelpers.Api(
                ts,
                "/api/projects",
                i < 400 ? 200 : 429,
                sessionId,
                anonId,
                duration: Random.Shared.Next(20, 200)
            );

            ts += TimeSpan.FromMilliseconds(Random.Shared.Next(5, 20));
        }
    }
}
