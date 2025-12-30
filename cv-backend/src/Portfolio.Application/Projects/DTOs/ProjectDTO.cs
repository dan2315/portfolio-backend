namespace Portfolio.Application.Projects.DTOs;

public record ProjectDTO(
string Id,
string Slug,
string Title,
string? ShortDescription,
string[]? Technologies,
string? Description,
int? PrideRating,
ProjectRepositoryDTO? Repository,
ProjectReactionsDTO Reactions,
bool IsPublished
);