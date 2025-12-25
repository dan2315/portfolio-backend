namespace Portfolio.Application.Projects.DTOs;

public record ProjectDTO(
string Slug,
string Title,
string? ShortDescription,
int? PrideRating,
ProjectRepositoryDTO? Repository,
ProjectReactionsDTO Reactions
);