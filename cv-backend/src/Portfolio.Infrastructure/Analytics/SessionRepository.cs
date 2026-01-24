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

    async Task<IReadOnlyList<ActivitySession>> ISessionRepository.GetAll()
    {
        return await _dbContext.ActivitySessions.ToListAsync();
    }
}