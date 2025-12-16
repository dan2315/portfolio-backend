using System.Text.Json;

public class LeetCodeService
{
    HttpClient _httpClient = new HttpClient();
    private const string LEETCODE_GRAPHQL_URL = "https://leetcode.com/graphql";

    public LeetCodeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LeetCodeData> GetUserStatsAsync(string username)
    {
        var query = @"
            query userProblemsSolved($username: String!) {
              allQuestionsCount {
                difficulty
                count
              }
              matchedUser(username: $username) {
                problemsSolvedBeatsStats {
                  difficulty
                  percentage
                }
                submitStatsGlobal {
                  acSubmissionNum {
                    difficulty
                    count
                  }
                }
              }
            }";
        
        var request = new
        {
            query,
            variables = new { username }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(LEETCODE_GRAPHQL_URL, content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LeetCodeResponse>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result.Data;
    }

    public async Task<LanguagesData> GetLanguagesData(string username)
    {
        var query = @"
            query languageStats($username: String!) {
                matchedUser(username: $username) {
                    languageProblemCount {
                    languageName
                    problemsSolved
                    }
                }
            }
        ";

        var request = new
        {
          query,
          variables = new { username }  
        };

        var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(LEETCODE_GRAPHQL_URL, content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LanguagesResponse>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        return result.Data;
    }

    public async Task<RecentSubmissionsData> GetRecentSubmissionsAsync(string username)
    {
        var query = @"
            query recentAcSubmissions($username: String!, $limit: Int!) {
            recentAcSubmissionList(username: $username, limit: $limit) {
                id
                title
                titleSlug
                timestamp
                }
            }";

        var request = new
        {
            query,
            variables = new { username, limit = 8 }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(LEETCODE_GRAPHQL_URL, content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LCRecentSubmissionsResponse>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result.Data;
    }

    public async Task<CalendarData> GetUserActivityCalendarAsync(string username) // TODO: Add year parameter
    {
        using var httpClient = new HttpClient();
        var query = @"
            query userProfileCalendar($username: String!, $year: Int) {
            matchedUser(username: $username) {
                userCalendar(year: $year) {
                activeYears
                streak
                totalActiveDays
                dccBadges {
                    timestamp
                    badge {
                    name
                    icon
                    }
                }
                submissionCalendar
                }
            }
            }";

        var request = new
        {
            query,
            variables = new { username }
        };

        var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(LEETCODE_GRAPHQL_URL, content);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Json response: " + jsonResponse);
        var result = JsonSerializer.Deserialize<LCActivityCalendar>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result.Data;
    }
}
