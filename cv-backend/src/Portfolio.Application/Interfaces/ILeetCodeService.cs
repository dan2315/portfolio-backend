using Portfolio.Application.Models;

public interface ILeetCodeService
{
    Task<LeetCodeProfile> GetProfileAsync(string username);
    Task<LeetCodeLanguages> GetLanguagesAsync(string username);
    Task<LeetCodeActivity> GetActivityAsync(string username, int? year);
    Task<LeetCodeSubmissions> GetSubmissionsAsync(string username, int limit);
}