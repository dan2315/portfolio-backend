using Portfolio.Application.Analytics.DTOs;

namespace Portfolio.Application.Analytics.Interfaces;

public interface ILiveSessionsStore
{
    public Task<IReadOnlyList<SessionDTO>> GetSessions();
    public Task<IReadOnlyList<SessionDTO>> GetSessions(DateTimeOffset from, DateTimeOffset to);
    public Task<IReadOnlyList<SessionDTO>> GetSessions(Guid userId, int limit);
    public Task StoreSessions(SessionDeltaState state);
    public Task<SessionsHeatmap> GetHeatmapAsync();
    public Task StoreHeatmapAsync();
}