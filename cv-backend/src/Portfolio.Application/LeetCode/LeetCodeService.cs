
using Portfolio.Application.Interfaces;
using Portfolio.Application.Models;

public class LeetCodeService : ILeetCodeService
{
    private readonly ILeetCodeClient _client;

    public LeetCodeService(ILeetCodeClient client)
    {
        _client = client;
    }

    public Task<LeetCodeActivity> GetActivityAsync(string username, int? year)
    {
        // var resolvedYear = year ?? DateTime.Now.Year;
        var resolvedYear = year ?? 2025;
        return _client.GetActivityAsync(username, resolvedYear);
    }

    public Task<LeetCodeLanguages> GetLanguagesAsync(string username)
    {
        return _client.GetLanguagesAsync(username);
    }

    public Task<LeetCodeProfile> GetProfileAsync(string username)
    {
        return _client.GetProfileAsync(username);
    }

    public Task<LeetCodeSubmissions> GetSubmissionsAsync(string username, int limit)
    {
        limit = Math.Clamp(limit, 1, 20);
        return _client.GetSubmissionsAsync(username, limit);
    }
}
