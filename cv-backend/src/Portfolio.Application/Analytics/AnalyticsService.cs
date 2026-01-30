using Microsoft.Extensions.Caching.Distributed;
using Portfolio.Application.Analytics.Interfaces;
using Portfolio.Application.Cache;

namespace Portfolio.Application.Analytics;

public class AnalyticsService : IAnalyticsService
{
    private ISessionRepository _sessionsRepository;
    private IDistributedCache _distributedCache;

    public AnalyticsService(IDistributedCache distributedCache ,ISessionRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
        _distributedCache = distributedCache;
    }

    public async Task<object> GetOrComputeSessionsHeatmap(int year)
    {
        var dailyActivity = await _sessionsRepository.GetDailyActivitiesAsyncBy(year);

        var heatmap = await _distributedCache.GetOrCreateAsync(
            key: $"heatmap:{year}",
            factory: async () =>
            {
                return dailyActivity
                    .Select(d => new
                    {
                        date = d.Date.ToString("yyyy/MM/dd"),
                        count = d.SessionsCount
                    })
                    .OrderBy(x => x.date)
                    .ToList();
            },
            ttl: TimeSpan.FromHours(24)
        );

        return heatmap!;
    }
}