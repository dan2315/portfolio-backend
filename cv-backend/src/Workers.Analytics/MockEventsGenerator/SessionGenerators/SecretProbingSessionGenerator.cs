using Portfolio.Application.Analytics;

namespace Workers.Analytics.MockEventsGenerator.SessionGenerators;

public sealed class SecretProbeSessionGenerator : ISessionPatternGenerator
{
    private static readonly string[] Probes =
    {
    "/.env",
    "/.git/config",
    "/config.yml",
    "/phpinfo.php",
    "/wp-admin"
};

    public IEnumerable<ActivityEvent> Generate(
        DateTimeOffset start,
        Guid sessionId,
        Guid anonId)
    {
        var ts = start;

        foreach (var route in Probes.OrderBy(_ => Random.Shared.Next()))
        {
            yield return FactoryHelpers.Api(
                ts,
                route,
                Random.Shared.Next(0, 2) == 0 ? 404 : 403,
                sessionId,
                anonId
            );

            ts += TimeSpan.FromMilliseconds(Random.Shared.Next(50, 150));
        }
    }
}