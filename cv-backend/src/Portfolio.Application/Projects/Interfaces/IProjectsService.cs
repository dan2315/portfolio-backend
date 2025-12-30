namespace Portfolio.Application.Projects.Interfaces;

using Portfolio.Application.Projects.DTOs;

public interface IProjectsService
{
    public Task<IReadOnlyList<ProjectDTO>> GetProjectsAsync(Guid anonSession);
    public Task<ProjectDTO?> GetProjectByIdAsync(Guid id, Guid anonSession);
    public Task<ProjectDTO?> GetProjectBySlugAsync(string slug, Guid anonSession);
    public Task CreateProject(CreateProjectDTO project);
    public Task<ProjectReactionsDTO> ToggleReactionAsync(string slug, string emoji, Guid id);
    public Task<ProjectDTO?> UpdateProjectAsync(Guid id, UpdateProjectDTO projectDTO);
    public Task<bool> DeleteProjectAsync(Guid id);
    public Task<bool> TogglePublishAsync(Guid id);
}