namespace Portfolio.Application.Projects.Interfaces;

using Portfolio.Domain.Entities;

public interface IProjectsRepository
{
    Task<IReadOnlyList<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(Guid id);
    Task AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Guid id);
    Task<Project?> GetBySlug(string slug);
    Task AddReaction(ProjectReaction reaction);
    Task DeleteReaction(ProjectReaction existingReaction);
}