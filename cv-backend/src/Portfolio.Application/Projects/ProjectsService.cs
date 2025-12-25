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
            var reactions = p.Reactions
                .GroupBy(r => r.Emoji)
                .ToDictionary(g => g.Key, g => g.Count());

            var selectedEmoji = p.Reactions
                .FirstOrDefault(r => r.AnonymousSessionId == anonSession)
                ?.Emoji ?? string.Empty;

            var reactionsDTO = new ProjectReactionsDTO(
                reactions,
                selectedEmoji,
                p.Slug
            );

            ProjectRepositoryDTO? repositoryDTO = null;
            if (!string.IsNullOrWhiteSpace(p.RepositoryUrl))
            {
                var repo = await _gitHubClient.GetRepository(p.RepositoryUrl);
                repositoryDTO = repo.MapIntoDTO();
            }

            return new ProjectDTO(
                p.Slug,
                p.Title,
                p.ShortDescription,
                p.PrideRating,
                repositoryDTO,
                reactionsDTO
            );
        }));

        return projectDTOs;
    }


    public async Task<DetailedProjectDTO?> GetProjectByIdAsync(Guid id)
    {
        var project = await _projectsRepository.GetByIdAsync(id);
        return project == null ? null : new DetailedProjectDTO(project.Title, project.ShortDescription, project.PrideRating);
    }

    public async Task<DetailedProjectDTO?> GetProjectBySlugAsync(string slug)
    {
        var project = await _projectsRepository.GetBySlug(slug);
        return project == null ? null : new DetailedProjectDTO(project.Title, project.ShortDescription, project.PrideRating);
    }
    
    public async Task CreateProject(CreateProjectDTO project)
    {
        await _projectsRepository.AddAsync(new Project
        {
            Id = Guid.CreateVersion7(),
            Title = project.Title,
            Slug = project.Title.Slugify(),
            Description = project.Description,
            PrideRating = project.PrideRating
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
}