namespace Portfolio.Infrastructure.LeetCode;

using System.Text;
using System.Text.Json;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Models;

public class LeetCodeClient : ILeetCodeClient
{
    private readonly HttpClient _httpClient;

    public LeetCodeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LeetCodeProfile> GetProfileAsync(string username)
    {
        var statsResponse = await this.ExecuteAsync<UserStatsResponse>(
            LeetCodeQueries.UserStats,
            new { username }
        );

        return new LeetCodeProfile
        {
            Username = username,
            SolvedEasy = statsResponse.Data.MatchedUser.SubmitStatsGlobal
                .AcSubmissionNum.First(x => x.Difficulty == "Easy").Count,
            SolvedMedium = statsResponse.Data.MatchedUser.SubmitStatsGlobal
                .AcSubmissionNum.First(x => x.Difficulty == "Medium").Count,
            SolvedHard = statsResponse.Data.MatchedUser.SubmitStatsGlobal
                .AcSubmissionNum.First(x => x.Difficulty == "Hard").Count
        };
    }

    public async Task<LeetCodeLanguages> GetLanguagesAsync(string username)
    {
        var langsResponse = await this.ExecuteAsync<LanguagesResponse>(
            LeetCodeQueries.LanguageStats,
            new { username }
        );

        return new LeetCodeLanguages
        {
            ProblemsSolvedByLanguages = [.. langsResponse.Data.MatchedUser.LanguageProblemCount
            .Select(x => new ProblemsSolvedByLanguage {LanguageName = x.LanguageName, ProblemsSolved = x.ProblemsSolved })]
        };
    }

    public async Task<LeetCodeActivity> GetActivityAsync(string username, int year)
    {
        var activityResponse = await this.ExecuteAsync<ActivityCalendarResponse>(
            LeetCodeQueries.UserProfileCalendar,
            new { username, year }
        );

        return new LeetCodeActivity
        {
            ActiveYears = activityResponse.Data.MatchedUser.UserCalendar.ActiveYears,
            Streak = activityResponse.Data.MatchedUser.UserCalendar.Streak,
            TotalActiveDays = activityResponse.Data.MatchedUser.UserCalendar.TotalActiveDays,
            SubmissionCalendar = activityResponse.Data.MatchedUser.UserCalendar.SubmissionCalendar
        };
    }

    public async Task<LeetCodeSubmissions> GetSubmissionsAsync(string username, int limit)
    {
        var submissionsResponse = await this.ExecuteAsync<RecentSubmissionsResponse>(
            LeetCodeQueries.RecentSubmissions,
            new { username, limit }
        );
        
        return new LeetCodeSubmissions
        {
            Submissions = submissionsResponse.Data.RecentAcSubmissionList
            .Select(x => new SubmissionItem {Id = x.Id, Timestamp = x.Timestamp, Title = x.Title, TitleSlug = x.TitleSlug}).ToArray()
        };
    }

    public async Task<T> ExecuteAsync<T>(string query, object variables)
    {
        var request = new
        {
            query,
            variables
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _httpClient.PostAsync("", content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions{PropertyNameCaseInsensitive = true})!;
    }
}