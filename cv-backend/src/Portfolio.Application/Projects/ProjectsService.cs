namespace Portfolio.Application.Projects;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Portfolio.Application.Projects.DTOs;
using Portfolio.Application.Projects.Interfaces;
using Portfolio.Domain.Entities;

public class ProjectsService : IProjectsService
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly IGitHubClient _gitHubClient;
    public ProjectsService(IProjectsRepository projectsRepository, IGitHubClient gitHubClient)
    {
        _projectsRepository = projectsRepository;
        _gitHubClient = gitHubClient;
    }

    public async Task<IReadOnlyList<ProjectDTO>> GetProjectsAsync(Guid anonSession)
    {
        var projects = await _projectsRepository.GetAllAsync();

        var projectDTOs = await Task.WhenAll(projects.Select(async p =>
        {
            var repositoryDTO = await TryGetRepositoryDataFor(p);
            return p.MapIntoDTO(anonSession, repositoryDTO);
        }));

        return projectDTOs;
    }


    public async Task<ProjectDTO?> GetProjectByIdAsync(Guid id, Guid anonSession)
    {
        var project = await _projectsRepository.GetByIdAsync(id);
        if (project == null) return null;
        var repositoryDTO = await TryGetRepositoryDataFor(project);
        return project.MapIntoDTO(anonSession, repositoryDTO);
    }

    public async Task<ProjectDTO?> GetProjectBySlugAsync(string slug, Guid anonSession)
    {
        var project = await _projectsRepository.GetBySlug(slug);
        if (project == null) return null;
        var repositoryDTO = await TryGetRepositoryDataFor(project);
        return project.MapIntoDTO(anonSession, repositoryDTO);
    }
    
    public async Task CreateProject(CreateProjectDTO project)
    {
        await _projectsRepository.AddAsync(new Project
        {
            Id = Guid.CreateVersion7(),
            Title = project.Title,
            Slug = project.Title.Slugify(),
            ShortDescription = project.ShortDescription,
            Description = project.Description,
            PrideRating = project.PrideRating,
            Technologies = project.Technologies,
            RepositoryUrl = project.RepositoryURL,
            IsPublished = project.IsPublished
        });
    }

    public async Task<ProjectReactionsDTO> ToggleReactionAsync(string slug, string emoji, Guid anonSession)
    {
        var project = await _projectsRepository.GetBySlug(slug) ?? 
        throw new Exception("Tried to add reaction: Project not found");

        var newReaction = new ProjectReaction
        {
            Emoji = emoji,
            AnonymousSessionId = anonSession,
            ProjectId = project.Id,
            CreatedAt = DateTime.UtcNow
        };

        Dictionary<string, int> reactions = project.Reactions
        .GroupBy(r => r.Emoji)
        .ToDictionary(g => g.Key, g => g.Count());

        ProjectReaction? existingReaction = project.Reactions.FirstOrDefault(r => r.AnonymousSessionId == anonSession);
        if (existingReaction?.Emoji == newReaction.Emoji)
        {
            await _projectsRepository.DeleteReaction(existingReaction);
            reactions[existingReaction.Emoji]--;
            if (reactions[existingReaction.Emoji] == 0) reactions.Remove(existingReaction.Emoji);
            return new ProjectReactionsDTO(reactions, string.Empty, project.Slug);
        }

        if (existingReaction != null)
        {
            await _projectsRepository.DeleteReaction(existingReaction);
            reactions[existingReaction.Emoji]--;
            if (reactions[existingReaction.Emoji] == 0) reactions.Remove(existingReaction.Emoji);
        }

        await _projectsRepository.AddReaction(newReaction);
        if (!reactions.TryGetValue(newReaction.Emoji, out int value)) reactions.Add(newReaction.Emoji, 1);
        else reactions[newReaction.Emoji] = ++value;
        return new ProjectReactionsDTO(reactions, newReaction.Emoji, project.Slug);
    }

    private async Task<ProjectRepositoryDTO?> TryGetRepositoryDataFor(Project project)
    {
        if (string.IsNullOrWhiteSpace(project.RepositoryUrl))
        return null;

        var repo = await _gitHubClient.GetRepository(project.RepositoryUrl);
        return repo.MapIntoDTO();
    }

    public async Task<ProjectDTO?> UpdateProjectAsync(Guid id, UpdateProjectDTO dto)
    {
        var project = await _projectsRepository.GetByIdAsync(id);

        if (project == null) return null;

        if (dto.Title is not null)
        {
            project.Title = dto.Title;
            project.Slug = dto.Title.Slugify();
        }

        if (dto.ShortDescription is not null)
            project.ShortDescription = dto.ShortDescription;

        if (dto.Description is not null)
            project.Description = dto.Description;

        if (dto.PrideRating.HasValue)
            project.PrideRating = dto.PrideRating;

        if (dto.Technologies is not null)
            project.Technologies = dto.Technologies;

        if (dto.RepositoryURL is not null)
            project.RepositoryUrl = dto.RepositoryURL;

        if (dto.IsPublished.HasValue)
            project.IsPublished = dto.IsPublished.Value;


        var repositoryDTO = await TryGetRepositoryDataFor(project);
        await _projectsRepository.UpdateAsync(project);
        return project.MapIntoDTO(repositoryDTO);
    }

    public async Task<bool> TogglePublishAsync(Guid id)
    {
        var project = await _projectsRepository.GetByIdAsync(id);
        if (project == null)
            return false;

        project.IsPublished = !project.IsPublished;

        await _projectsRepository.UpdateAsync(project);
        return true;
    }

    public async Task<bool> DeleteProjectAsync(Guid id)
    {
        await _projectsRepository.DeleteAsync(id);
        return true;
    }
}