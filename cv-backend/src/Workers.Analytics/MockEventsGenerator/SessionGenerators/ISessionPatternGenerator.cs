using Portfolio.Application.Analytics;

public interface ISessionPatternGenerator
{
    IEnumerable<ActivityEvent> Generate(
        DateTimeOffset startTime,
        Guid sessionId,
        Guid anonId
    );
}
