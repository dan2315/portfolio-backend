using System.Collections.Concurrent;
using Portfolio.Infrastructure.GitHub.DTOs;

namespace Portfolio.Infrastructure.GitHub
{
    public class GitHubIds
    {
        private readonly HttpClient _httpClient;
        private ConcurrentDictionary<string, string> _ids = new();
        public GitHubIds(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
 
        public async Task<string> GetFor(string username)
        {
            if (_ids.ContainsKey(username)) return _ids[username];

            var response = await _httpClient.ExecuteGraphqlAsync<UserResponseData>(
                GitHubQuerries.GetUserId,
                new { owner = username }
            );

            return _ids.GetOrAdd(username, response.User.Id);
        }
    
    }
}