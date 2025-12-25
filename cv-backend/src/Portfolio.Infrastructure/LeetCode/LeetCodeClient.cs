namespace Portfolio.Infrastructure.LeetCode;

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
        var statsResponse = await _httpClient.ExecuteGraphqlAsync<UserStatsData>(
            LeetCodeQueries.UserStats,
            new { username }
        );

        return new LeetCodeProfile
        {
            Username = username,
            SolvedEasy = statsResponse.MatchedUser.SubmitStatsGlobal
                .AcSubmissionNum.First(x => x.Difficulty == "Easy").Count,
            SolvedMedium = statsResponse.MatchedUser.SubmitStatsGlobal
                .AcSubmissionNum.First(x => x.Difficulty == "Medium").Count,
            SolvedHard = statsResponse.MatchedUser.SubmitStatsGlobal
                .AcSubmissionNum.First(x => x.Difficulty == "Hard").Count
        };
    }

    public async Task<LeetCodeLanguages> GetLanguagesAsync(string username)
    {
        var langsResponse = await _httpClient.ExecuteGraphqlAsync<LanguagesData>(
            LeetCodeQueries.LanguageStats,
            new { username }
        );

        return new LeetCodeLanguages
        {
            ProblemsSolvedByLanguages = [.. langsResponse.MatchedUser.LanguageProblemCount
            .Select(x => new ProblemsSolvedByLanguage {LanguageName = x.LanguageName, ProblemsSolved = x.ProblemsSolved })]
        };
    }

    public async Task<LeetCodeActivity> GetActivityAsync(string username, int year)
    {
        var activityResponse = await _httpClient.ExecuteGraphqlAsync<CalendarData>(
            LeetCodeQueries.UserProfileCalendar,
            new { username, year }
        );

        return new LeetCodeActivity
        {
            ActiveYears = activityResponse.MatchedUser.UserCalendar.ActiveYears,
            Streak = activityResponse.MatchedUser.UserCalendar.Streak,
            TotalActiveDays = activityResponse.MatchedUser.UserCalendar.TotalActiveDays,
            SubmissionCalendar = activityResponse.MatchedUser.UserCalendar.SubmissionCalendar
        };
    }

    public async Task<LeetCodeSubmissions> GetSubmissionsAsync(string username, int limit)
    {
        var submissionsResponse = await _httpClient.ExecuteGraphqlAsync<RecentSubmissionsData>(
            LeetCodeQueries.RecentSubmissions,
            new { username, limit }
        );
        
        return new LeetCodeSubmissions
        {
            Submissions = submissionsResponse.RecentAcSubmissionList
            .Select(x => new SubmissionItem {Id = x.Id, Timestamp = x.Timestamp, Title = x.Title, TitleSlug = x.TitleSlug}).ToArray()
        };
    }


}