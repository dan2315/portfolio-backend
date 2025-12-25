namespace Portfolio.Application.Projects.Interfaces;

using Portfolio.Application.Projects.DTOs;

public interface IProjectsService
{
    public Task<IReadOnlyList<ProjectDTO>> GetProjectsAsync(Guid id);
    public Task<DetailedProjectDTO?> GetProjectByIdAsync(Guid id);
    public Task<DetailedProjectDTO?> GetProjectBySlugAsync(string slug);
    public Task CreateProject(CreateProjectDTO project);
    public Task<ProjectReactionsDTO> ToggleReactionAsync(string slug, string emoji, Guid id);
}