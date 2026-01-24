using Portfolio.Application.Analytics;

namespace Workers.Analytics.MockEventsGenerator.SessionGenerators;

public sealed class AdminBruteSessionGenerator : ISessionPatternGenerator
{
    private static readonly string[] Routes =
    {
    "/admin",
    "/admin/login",
    "/api/admin/users"
};

    public IEnumerable<ActivityEvent> Generate(
        DateTimeOffset start,
        Guid sessionId,
        Guid anonId)
    {
        var ts = start;

        for (int i = 0; i < Random.Shared.Next(20, 100); i++)
        {
            yield return FactoryHelpers.Api(
                ts,
                Routes[Random.Shared.Next(Routes.Length)],
                403,
                sessionId,
                anonId
            );

            ts += TimeSpan.FromMilliseconds(Random.Shared.Next(20, 80));
        }
    }
}