public static class LeetCodeQueries
{
    public const string UserStats = """
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
      }
    """;

    public const string LanguageStats = """
      query languageStats($username: String!) {
        matchedUser(username: $username) {
          languageProblemCount {
            languageName
            problemsSolved
          }
        }
      }
    """;

    public const string RecentSubmissions = """
      query recentAcSubmissions($username: String!, $limit: Int!) {
      recentAcSubmissionList(username: $username, limit: $limit) {
          id
          title
          titleSlug
          timestamp
          }
      }
    """;

    public const string UserProfileCalendar = """
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
      }
    """;
}
