using Portfolio.Application.Projects.DTOs;
using Portfolio.Domain.Entities;

public static class ProjectTypeMapper
{
    public static ProjectDTO MapIntoDTO(this Project p, ProjectRepositoryDTO? repositoryDTO = null)
    {
        return p.MapIntoDTO(new Guid(), repositoryDTO);
    }

    public static ProjectDTO MapIntoDTO(this Project p, Guid anonSession, ProjectRepositoryDTO? repositoryDTO = null)
    {
        var reactions = p.Reactions
            .GroupBy(r => r.Emoji)
            .ToDictionary(g => g.Key, g => g.Count());

        var selectedEmoji = p.Reactions
            .FirstOrDefault(r => r.AnonymousId == anonSession)
            ?.Emoji ?? string.Empty;

        var reactionsDTO = new ProjectReactionsDTO(
            reactions,
            selectedEmoji,
            p.Slug
        );

        return new ProjectDTO(
            p.Id.ToString(),
            p.Slug,
            p.Title,
            p.ShortDescription,
            p.Technologies,
            p.Description,
            p.PrideRating,
            repositoryDTO,
            reactionsDTO,
            p.IsPublished
        );
    }
}