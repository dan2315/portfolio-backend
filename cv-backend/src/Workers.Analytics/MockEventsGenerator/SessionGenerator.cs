using Portfolio.Application.Analytics;
using Workers.Analytics.MockEventsGenerator;
using Workers.Analytics.MockEventsGenerator.SessionGenerators;

public sealed class SessionGenerator
{
    private readonly Random _rnd = Random.Shared;

    private readonly NormalSessionGenerator _normal = new();
    private readonly AdminBruteSessionGenerator _admin = new();
    private readonly SecretProbeSessionGenerator _secrets = new();
    private readonly FastNavigationSessionGenerator _fast = new();
    private readonly ApiFloodSessionGenerator _flood = new();

    public IEnumerable<ActivityEvent> GenerateSession(
        DateTimeOffset startTime,
        Guid sessionId,
        Guid anonId)
    {
        var kind = PickSessionKind();

        return kind switch
        {
            SessionKind.Normal => _normal.Generate(startTime, sessionId, anonId),
            SessionKind.AdminBrute => _admin.Generate(startTime, sessionId, anonId),
            SessionKind.SecretProbe => _secrets.Generate(startTime, sessionId, anonId),
            SessionKind.FastNavigation => _fast.Generate(startTime, sessionId, anonId),
            SessionKind.ApiFlood => _flood.Generate(startTime, sessionId, anonId),
            _ => _normal.Generate(startTime, sessionId, anonId)
        };
    }

    private SessionKind PickSessionKind()
    {
        var r = _rnd.NextDouble();

        if (r < 0.80) return SessionKind.Normal;
        if (r < 0.88) return SessionKind.AdminBrute;
        if (r < 0.92) return SessionKind.SecretProbe;
        if (r < 0.97) return SessionKind.FastNavigation;
        return SessionKind.ApiFlood;
    }
}
