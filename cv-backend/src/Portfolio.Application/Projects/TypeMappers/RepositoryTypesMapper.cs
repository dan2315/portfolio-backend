using Portfolio.Application.Projects.DTOs;
using Portfolio.Domain.Entities;
using Commit = Portfolio.Application.Projects.DTOs.Commit;
using DefaultBranchRef = Portfolio.Application.Projects.DTOs.DefaultBranchRef;
using Language = Portfolio.Application.Projects.DTOs.Language;
using Release = Portfolio.Application.Projects.DTOs.Release;
using RepositoryDynamicData = Portfolio.Application.Projects.DTOs.RepositoryDynamicData;

public static class RepositoryTypesMapper
{
    public static ProjectRepositoryDTO MapIntoDTO(this ProjectRepository repo) 
    {
        if (repo == null) throw new ArgumentNullException(nameof(repo));

        return new ProjectRepositoryDTO
        {
            Name = repo.Name,
            Description = repo.Description,
            Url = repo.Url,
            HomepageUrl = repo.HomepageUrl,
            PrimaryLanguage = repo.PrimaryLanguage != null 
                ? new Language 
                { 
                    Name = repo.PrimaryLanguage.Name, 
                    Color = repo.PrimaryLanguage.Color 
                } 
                : null,
            Languages = repo.Languages?.Select(l => new Language
            {
                Name = l.Name,
                Color = l.Color
            }).ToList() ?? new List<Language>(),
            DefaultBranchRef = repo.DefaultBranchRef != null
                ? new DefaultBranchRef
                {
                    Name = repo.DefaultBranchRef.Name,
                    Oid = repo.DefaultBranchRef.Oid,
                    Commits = repo.DefaultBranchRef.Commits?.Select(c => new Commit
                    {
                        Message = c.Message,
                        CommittedDate = c.CommittedDate,
                        Oid = c.Oid
                    }).ToList() ?? []
                }
                : null,
            ReadmeMarkdown = repo.ReadmeMarkdown,
            DynamicData = repo.DynamicData != null 
                ? new RepositoryDynamicData
                {
                    ForkCount = repo.DynamicData.ForkCount,
                    Issues = repo.DynamicData.Issues,
                    PullRequests = repo.DynamicData.PullRequests,
                    StargazerCount = repo.DynamicData.StargazerCount,
                    Releases = repo.DynamicData.Releases?.Select(r => new Release{TagName = r.TagName, PublishedAt = r.PublishedAt}).ToList() ?? [],
                    Watchers = repo.DynamicData.Watchers
                } : null

        };
    }
}
