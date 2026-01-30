using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Analytics;

public class SessionRepository : ISessionRepository
{
    private readonly AppDbContext _dbContext;

    public SessionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ActivitySession>> GetAll()
    {
        return await _dbContext.ActivitySessions.ToListAsync();
    }

    public async Task<IReadOnlyList<DailyActivity>> GetDailyActivitiesAsyncBy(int year)
    {
        var start = new DateTime(year, 1, 1).ToUniversalTime();
        var end = start.AddYears(1).ToUniversalTime();

        return await _dbContext.DailyActivity
            .AsNoTracking()
            .Where(d => d.Date >= start && d.Date < end)
            .ToListAsync();
    }
}