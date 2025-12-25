using Portfolio.Domain.Entities;

public interface IGitHubClient
{
    Task<ProjectRepository> GetRepository(string githubUrl);
}