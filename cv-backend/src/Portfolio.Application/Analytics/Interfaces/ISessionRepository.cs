using System.Diagnostics;
using Portfolio.Domain.Entities;

public interface ISessionRepository
{
    Task<IReadOnlyList<ActivitySession>> GetAll();
    Task<IReadOnlyList<DailyActivity>> GetDailyActivitiesAsyncBy(int year);
}