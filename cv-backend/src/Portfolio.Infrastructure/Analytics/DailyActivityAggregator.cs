using Microsoft.EntityFrameworkCore;
using Portfolio.Infrastructure.Entities;
using Portfolio.Infrastructure.Persistence;

namespace Portfolio.Infrastructure.Analytics;

public class DailyActivityAggregator // TODO: Decouple use case to Application layer
{
    private readonly AppDbContext _dbContext;

    public DailyActivityAggregator(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AggregateAsync(DateOnly day, CancellationToken ct)
    {
        var dayStart = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);
        var safeStart = dayStart.AddMinutes(-30);
        var safeEnd = dayEnd.AddMinutes(-30);

        var sql = @"
        INSERT INTO daily_activity (""Date"", ""SessionsCount"", ""PageViews"", ""AverageSessionDurationMs"", ""UniqueUsersCount"")
        SELECT
            DATE(""EndTime"") AS ""Date"",
            COUNT(DISTINCT ""SessionId"") AS ""SessionsCount"",
            SUM(""PagesViewed"") AS ""PageViews"",
            CASE WHEN COUNT(*) = 0 THEN 0 ELSE SUM(""TotalTimeMs"") / COUNT(*) END AS ""AverageSessionDurationMs"",
            COUNT(DISTINCT ""AnonymousId"") AS ""UniqueUsersCount""
        FROM activity_sessions
        WHERE ""EndTime"" >= @safeStart
        AND ""EndTime"" < @safeEnd
        GROUP BY DATE(""EndTime"")
        ON CONFLICT (""Date"") DO UPDATE SET
            ""SessionsCount"" = EXCLUDED.""SessionsCount"",
            ""PageViews"" = EXCLUDED.""PageViews"",
            ""AverageSessionDurationMs"" = EXCLUDED.""AverageSessionDurationMs"",
            ""UniqueUsersCount"" = EXCLUDED.""UniqueUsersCount"";
        ";

        await _dbContext.Database.ExecuteSqlRawAsync(sql,
        [
            new Npgsql.NpgsqlParameter("safeStart", safeStart),
            new Npgsql.NpgsqlParameter("safeEnd", safeEnd)
        ], ct);
    }

    public async Task AggregatePendingAsync(CancellationToken ct)
    {
        var progress = await _dbContext.DailyActivityProgresses.SingleOrDefaultAsync(ct);

        var startDay = progress?.LastAggregatedDay.AddDays(1) ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        for (var day = startDay; day <= yesterday; day = day.AddDays(1))
        {
            await AggregateAsync(day, ct);

            if (progress is null)
            {
                progress = new DailyActivityProgress
                {
                    LastAggregatedDay = day
                };
                _dbContext.DailyActivityProgresses.Add(progress);
            }
            else
            {
                progress.LastAggregatedDay = day;
                _dbContext.DailyActivityProgresses.Update(progress);
            }

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}