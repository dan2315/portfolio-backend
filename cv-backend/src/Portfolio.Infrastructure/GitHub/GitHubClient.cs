using Microsoft.Extensions.Caching.Memory;
using Portfolio.Domain.Entities;
using Portfolio.Infrastructure.GitHub;
using Portfolio.Infrastructure.GitHub.Cache;
using RepositoryDynamicData = Portfolio.Infrastructure.GitHub.DTOs.RepositoryDynamicData;

public class GitHubClient : IGitHubClient
{
    private readonly HttpClient _httpClient;
    private readonly ParsedUrlCache<GitHubClient> _urlCache;
    private readonly GitHubIds _gitHubIds;
    private readonly IMemoryCache _cache;
    public GitHubClient(HttpClient httpClient, ParsedUrlCache<GitHubClient> urlCache, GitHubIds gitHubIds, IMemoryCache memoryCache)
    {
        _httpClient = httpClient;
        _urlCache = urlCache;
        _gitHubIds = gitHubIds;
        _cache = memoryCache;
    }

    public async Task<ProjectRepository> GetRepository(string githubUrl)
    {
        (string owner, string name) = _urlCache.TryGet(githubUrl, ParseGithubUrl);
        string id = await _gitHubIds.GetFor(owner);
        
        var response = await GetStaticRepositoryData(owner, name, id);
        var responseDynamicData = await GetDynamicRepositoryData(owner, name);
        
        var projectRepository = new ProjectRepository(
            response.Repository.Name,
            response.Repository.Description,
            response.Repository.Url,
            response.Repository.HomepageUrl,
            new Portfolio.Domain.Entities.Language{
                Name = response.Repository.PrimaryLanguage?.Name,
                Color = response.Repository.PrimaryLanguage?.Color },
            response.Repository.Languages.Edges.Select(l => new Portfolio.Domain.Entities.Language
            {
                Name = l.Node.Name,
                Color = l.Node.Color
            }).ToList(),
            new Portfolio.Domain.Entities.DefaultBranchRef
            {Name = response.Repository.DefaultBranchRef.Name,
            Oid = response.Repository.DefaultBranchRef.Target.Oid,
            Commits = response.Repository.DefaultBranchRef.Target.History.Edges.Select(e => 
                new Commit { Message = e.Node.Message, CommittedDate = e.Node.CommittedDate, Oid = e.Node.Oid }
                ).ToList()
            },
            response.Repository.Object?.Text // README.md
        );


        projectRepository.DynamicData = new Portfolio.Domain.Entities.RepositoryDynamicData
        {
            StargazerCount = responseDynamicData.Repository.StargazerCount,
            ForkCount = responseDynamicData.Repository.ForkCount,
            Issues = responseDynamicData.Repository.Issues.TotalCount,
            PullRequests = responseDynamicData.Repository.PullRequests.TotalCount,
            Watchers = responseDynamicData.Repository.Watchers.TotalCount,
            Releases = responseDynamicData.Repository.Releases.Nodes.Select(r => 
                new Release { TagName = r.TagName, PublishedAt = r.PublishedAt}).ToList(),
        };

        return projectRepository;
    }

    private async Task<RepositoryData> GetStaticRepositoryData(string owner, string name, string id)
    {
        var cacheKey = $"github:repo:static:{owner}/{name}";

        var cachedResponse = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(8);

            return await _httpClient.ExecuteGraphqlAsync<RepositoryData>(
                GitHubQuerries.RepoStaticDataQuery,
                new { owner, name, id }
            );
        }) ?? throw new Exception("PIZDAAAA"); // TODO
        return cachedResponse;
    }

    private async Task<RepositoryDynamicData> GetDynamicRepositoryData(string owner, string name)
    {
        var cacheKey = $"github:repo:dynamic:{owner}/{name}";

        var cachedResponse = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);

            return await _httpClient.ExecuteGraphqlAsync<RepositoryDynamicData>(
                GitHubQuerries.RepoDynamicDataQuery,
                new { owner, name }
            );
        }) ?? throw new Exception("PIZDAAAA");
        return cachedResponse;
    }


    private Tuple<string, string> ParseGithubUrl(string githubUrl)
    {
        githubUrl = githubUrl.TrimEnd('/');

        var uri = new Uri(githubUrl);
        if (uri.Host != "github.com")
            throw new ArgumentException("GitHub URL is invalid");

        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length < 2)
            throw new ArgumentException("GitHub URL is invalid");

        string username = segments[0];
        string repoName = segments[1];

        return new Tuple<string, string>(username, repoName);
    }
}