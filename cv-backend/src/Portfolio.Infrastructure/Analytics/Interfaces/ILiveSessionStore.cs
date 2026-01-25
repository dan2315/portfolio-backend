using Portfolio.Application.Analytics.DTOs;
using Workers.Analytics;

namespace Portfolio.Infrastructure.Analytics.Interfaces;

public interface ILiveSessionsStore
{
    public Task<IReadOnlyList<SessionDTO>> GetSessions();
    public Task<IReadOnlyList<SessionDTO>> GetSessions(DateTimeOffset from, DateTimeOffset to);
    public Task<IReadOnlyList<SessionDTO>> GetSessions(Guid userId, int limit);
    public Task StoreSessions(SessionDeltaState state);
}